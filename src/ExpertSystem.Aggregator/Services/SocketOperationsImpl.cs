using System;
using Grpc.Core;
using System.Threading.Tasks;
using ExpertSystem.Aggregator.Processors;
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
        private readonly ProductionProcessor _productionProcessor;
        private readonly LogicProcessor _logicProcessor;
        private readonly FuzzyProcessor _fuzzyProcessor;
        private readonly NeuralProcessor _neuralProcessor;
        private readonly SocketOperationsOptions _options;

        public SocketOperationsImpl(ProductionProcessor productionProcessor, LogicProcessor logicProcessor,
            FuzzyProcessor fuzzyProcessor, NeuralProcessor neuralProcessor, SocketOperationsOptions options)
        {
            _productionProcessor = productionProcessor;
            _logicProcessor = logicProcessor;
            _fuzzyProcessor = fuzzyProcessor;
            _neuralProcessor = neuralProcessor;
            _options = options;
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
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
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