using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Grpc.Core;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Tests.Services
{
    public class SocketExchangeMock : SocketExchange.SocketExchangeBase
    {
        private readonly List<CustomSocket> _sockets;

        public SocketExchangeMock(List<CustomSocket> sockets)
        {
            _sockets = sockets;
        }

        public override Task GetSockets(Empty request, IServerStreamWriter<CustomSocket> responseStream,
            ServerCallContext context)
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
    }
}