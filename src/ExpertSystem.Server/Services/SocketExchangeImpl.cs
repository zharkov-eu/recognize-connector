using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Server.Parsers
{
    class SocketExchangeImpl : SocketExchange.SocketExchangeBase
    {
        private List<CustomSocket> _sockets;

        public SocketExchangeImpl(List<CustomSocket> sockets)
        {
            _sockets = sockets;
        }

        public override Task GetSockets(Empty request, IServerStreamWriter<CustomSocket> responseStream, ServerCallContext context)
        {
            Task.WaitAll(_sockets.Select(it => responseStream.WriteAsync(it)).ToArray());
            return Task.CompletedTask;
        }
    }
}