using System;
using Grpc.Core;
using System.Threading.Tasks;
using ExpertSystem.Aggregator.Processors;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Common.Generated;

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

        public override Task FindSocketsByParams(CustomSocket request, IServerStreamWriter<CustomSocket> responseStream,
            ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task<CustomSocket> FindSocketByIdentity(CustomSocketIdentity request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task<CustomSocket> IsParamsMatchSocket(CustomSocket request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task GetSocketGroups(Empty request, IServerStreamWriter<SocketGroup> responseStream,
            ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task GetSocketsInGroup(SocketGroupIdentity request,
            IServerStreamWriter<CustomSocket> responseStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task<CustomSocket> CheckSocketInGroup(CustomSocketIdentityJoinGroup request,
            ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task FindSocketsByParamsInGroup(CustomSocketJoinGroup request,
            IServerStreamWriter<CustomSocketJoinGroup> responseStream, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task<CustomSocket> UpsertSocket(CustomSocket request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
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
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public Task<SocketGroup> AddToSocketGroup(SocketGroup request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        public override Task<Empty> DeleteSocketGroup(SocketGroupIdentity request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
        }

        private void DebugWrite(string message)
        {
            if (_options.Debug)
                Console.WriteLine(message);
        }
    }
}