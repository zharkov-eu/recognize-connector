using System;
using System.Linq;
using Grpc.Core;
using System.Threading.Tasks;
using ExpertSystem.Aggregator.Exceptions;
using ExpertSystem.Aggregator.Processors;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Aggregator.Services
{
    public struct SocketOperationsOptions
    {
        public string Version;
        public bool Debug;
    }

    public class SocketOperationsImpl : SocketOperations.SocketOperationsBase
    {
        private readonly EntityCache<CustomSocket> _socketCache;
        private readonly EntityCache<SocketGroup> _socketGroupCache;
        private readonly SocketExchange.SocketExchangeClient _client;
        private readonly ProductionProcessor _productionProcessor;
        private readonly LogicProcessor _logicProcessor;
        private readonly FuzzyProcessor _fuzzyProcessor;
        private readonly NeuralProcessor _neuralProcessor;
        private readonly SocketOperationsOptions _options;

        public SocketOperationsImpl(EntityCache<CustomSocket> socketCache, EntityCache<SocketGroup> socketGroupCache,
            SocketExchange.SocketExchangeClient client, SocketOperationsOptions options)
        {
            _client = client;
            _socketCache = socketCache;
            _socketGroupCache = socketGroupCache;
            _options = options;

            var rulesGenerator = new ProductionRulesGenerator();
            var logicRulesGenerator = new LogicRulesGenerator();
            var fuzzyRulesGenerator = new FuzzyRulesGenerator();
            var neuralRulesGenerator = new NeuralRulesGenerator();

            // Продукционный вывод
            var rulesGraph = rulesGenerator.GenerateRules(_socketCache.GetAll());
            // Логический вывод
            var logicRules = logicRulesGenerator.GenerateRules(_socketCache.GetAll());
            // Нечеткий вывод
            var fuzzyDomains = fuzzyRulesGenerator.GetFuzzyDomains(_socketCache.GetAll());
            var fuzzyFacts = fuzzyRulesGenerator.GetFuzzyFacts(fuzzyDomains, _socketCache.GetAll());
            // Нейро-нечеткий вывод
            var neuralNetwork = neuralRulesGenerator.GetNeuralNetwork(_socketCache.GetAll());

            var processorOptions = new ProcessorOptions {Debug = _options.Debug};
            _productionProcessor = new ProductionProcessor(rulesGraph, processorOptions);
            _logicProcessor = new LogicProcessor(logicRules, processorOptions);
            _fuzzyProcessor = new FuzzyProcessor(fuzzyDomains, fuzzyFacts, processorOptions);
            _neuralProcessor = new NeuralProcessor(neuralNetwork, processorOptions);
        }

        public override Task<HelloMessage> SayHello(HelloMessage request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'SayHello': '{request}' from {context.Peer}");

            return Task.FromResult(new HelloMessage {Version = _options.Version});
        }

        public override async Task FindSocketsByParams(CustomSocket request,
            IServerStreamWriter<CustomSocket> responseStream,
            ServerCallContext context)
        {
            DebugWrite($"RpcCall 'FindSocketsByParams': '{request}' from {context.Peer}");

            var socketNames = _productionProcessor.ForwardProcessing(SocketToFactSet(request));
            var sockets = socketNames.Select(p => _socketCache.Get(p));

            foreach (var socket in sockets)
                await responseStream.WriteAsync(socket);
        }

        public override Task<CustomSocket> FindSocketByIdentity(CustomSocketIdentity request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'FindSocketByIdentity': '{request}' from {context.Peer}");

            try
            {
                var factSet = _productionProcessor.BackProcessing(request.SocketName);
                return Task.FromResult(FactSetToSocket(factSet));
            }
            catch (Exception exception)
            {
                if (exception is EntityNotFoundException)
                    throw new RpcException(new Status(StatusCode.NotFound, exception.Message));
                throw new RpcException(new Status(StatusCode.Aborted, "BackProcessing throws unhandled exception"));
            }
        }

        public override Task<CustomSocket> IsParamsMatchSocket(CustomSocket request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'IsParamsMatchSocket': '{request}' from {context.Peer}");

            if (request.SocketName == null)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "SocketName must be provided"));

            var isResolved = _logicProcessor.Processing(SocketToFactSet(request), request.SocketName);
            return isResolved
                ? Task.FromResult(_socketCache.Get(request.SocketName))
                : Task.FromResult(new CustomSocket());
        }

        public override Task<CustomSocketAmperage> FuzzyGetAmperageCircuitByParams(FuzzySocketRequest request,
            ServerCallContext context)
        {
            DebugWrite($"RpcCall 'FuzzyGetAmperageCircuitByParams': '{request}' from {context.Peer}");

            var socketAmperage = new CustomSocketAmperage();
            switch (request.Method)
            {
                case FuzzyMethod.Mamdani:
                    socketAmperage.AmperageCircuit =
                        _fuzzyProcessor.MamdaniProcessing(SocketParamsToFactSet(request.Socket));
                    break;
                case FuzzyMethod.Sugeno:
                    socketAmperage.AmperageCircuit =
                        _fuzzyProcessor.SugenoProcessing(SocketParamsToFactSet(request.Socket));
                    break;
                case FuzzyMethod.Neural:
                    socketAmperage.AmperageCircuit =
                        _neuralProcessor.Process(SocketParamsToFactSet(request.Socket));
                    break;
                default:
                    throw new RpcException(new Status(StatusCode.FailedPrecondition,
                        $"Method must be one of '{FuzzyMethod.Mamdani}', '{FuzzyMethod.Sugeno}', '{FuzzyMethod.Neural}'"
                    ));
            }

            return Task.FromResult(socketAmperage);
        }

        public override async Task GetSocketGroups(Empty request, IServerStreamWriter<SocketGroup> responseStream,
            ServerCallContext context)
        {
            DebugWrite($"RpcCall 'GetSocketGroups': '{request}' from {context.Peer}");

            foreach (var socketGroup in _socketGroupCache.GetAll())
                await responseStream.WriteAsync(socketGroup);
        }

        public override async Task GetSocketsInGroup(SocketGroupIdentity request,
            IServerStreamWriter<CustomSocket> responseStream, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'GetSocketsInGroup': '{request}' from {context.Peer}");

            if (!_socketGroupCache.EntityExists(request.GroupName))
                throw new RpcException(new Status(StatusCode.NotFound, $"SocketGroup {request.GroupName} not found"));
            var socketGroup = _socketGroupCache.Get(request.GroupName);

            foreach (var socketName in socketGroup.SocketNames)
                await responseStream.WriteAsync(_socketCache.Get(socketName));
        }

        public override Task<CustomSocket> CheckSocketInGroup(CustomSocketIdentityJoinGroup request,
            ServerCallContext context)
        {
            DebugWrite($"RpcCall 'CheckSocketInGroup': '{request}' from {context.Peer}");

            if (!_socketGroupCache.EntityExists(request.Group.GroupName))
                throw new RpcException(
                    new Status(StatusCode.NotFound, $"SocketGroup {request.Group.GroupName} not found")
                );
            var socketGroup = _socketGroupCache.Get(request.Group.GroupName);

            if (!socketGroup.SocketNames.Contains(request.Socket.SocketName))
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Socket {request.Socket.SocketName} in SocketGroup {request.Group.GroupName} not found")
                );

            return Task.FromResult(_socketCache.Get(request.Socket.SocketName));
        }

        public override Task FindSocketsByParamsInGroup(CustomSocketJoinGroup request,
            IServerStreamWriter<CustomSocketJoinGroup> responseStream, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'FindSocketsByParamsInGroup': '{request}' from {context.Peer}");
            
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task<CustomSocket> UpsertSocket(CustomSocket request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'UpsertSocket': '{request}' from {context.Peer}");

            var socket = new CustomSocket();
            if (_socketCache.EntityExists(request.SocketName))
                socket = _socketCache.Get(request.SocketName);
            socket.MergeFrom(request);

            _client.UpsertSocket(socket);
            _socketCache.Upsert(socket);

            return Task.FromResult(socket);
        }

        public override Task<Empty> DeleteSocket(CustomSocketIdentity request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'DeleteSocket': '{request}' from {context.Peer}");

            if (!_socketCache.EntityExists(request.SocketName))
                throw new RpcException(new Status(StatusCode.NotFound, $"Socket {request.SocketName} not found"));
            _client.DeleteSocket(new CustomSocketIdentity {SocketName = request.SocketName});
            _socketCache.Remove(request.SocketName);
            return Task.FromResult(new Empty());
        }

        public override Task<SocketGroup> AddSocketGroup(SocketGroupIdentity request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'AddSocketGroup': '{request}' from {context.Peer}");

            if (_socketGroupCache.EntityExists(request.GroupName))
                throw new RpcException(
                    new Status(StatusCode.AlreadyExists, $"SocketGroup {request.GroupName} already exists")
                );

            var socketGroup = _client.AddSocketGroup(new SocketGroupIdentity {GroupName = request.GroupName});
            _socketGroupCache.Add(socketGroup);

            return Task.FromResult(socketGroup);
        }

        public override Task<SocketGroup> AddToSocketGroup(CustomSocketIdentityJoinGroup request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'AddToSocketGroup': '{request}' from {context.Peer}");
            
            return base.AddToSocketGroup(request, context);
        }

        public override Task<SocketGroup> RemoveFromSocketGroup(CustomSocketIdentityJoinGroup request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'RemoveFromSocketGroup': '{request}' from {context.Peer}");
            
            return base.RemoveFromSocketGroup(request, context);
        }

        public override Task<Empty> DeleteSocketGroup(SocketGroupIdentity request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'DeleteSocketGroup': '{request}' from {context.Peer}");

            if (!_socketGroupCache.EntityExists(request.GroupName))
                throw new RpcException(new Status(StatusCode.NotFound, $"SocketGroup {request.GroupName} not found"));
            _client.DeleteSocketGroup(new SocketGroupIdentity {GroupName = request.GroupName});
            _socketGroupCache.Remove(request.GroupName);
            return Task.FromResult(new Empty());
        }

        private void DebugWrite(string message)
        {
            if (_options.Debug)
                Console.WriteLine(message);
        }

        private static FactSet SocketParamsToFactSet(FuzzySocketParams socket)
        {
            return new FactSet(
                new Fact(SocketDomain.NumberOfContacts, socket.NumberOfContacts),
                new Fact(SocketDomain.SizeLength, socket.SizeLength),
                new Fact(SocketDomain.SizeWidth, socket.SizeWidth)
            );
        }

        private static FactSet SocketToFactSet(CustomSocket socket)
        {
            var factSet = new FactSet();
            foreach (var domain in GetSocketDomains())
            {
                var value = CustomSocketExtension.SocketType.GetProperty(domain.ToString()).GetValue(socket);
                if (value != SocketDefaultValue[SocketDomainType[domain]])
                    factSet.Add(new Fact(domain, value));
            }

            return factSet;
        }

        private static CustomSocket FactSetToSocket(FactSet factSet)
        {
            var socket = new CustomSocket();
            foreach (var fact in factSet)
                CustomSocketExtension.SocketType.GetProperty(fact.Domain.ToString()).SetValue(socket, fact.Value);
            return socket;
        }
    }
}