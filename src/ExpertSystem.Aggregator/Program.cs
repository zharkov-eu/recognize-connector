using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using ExpertSystem.Aggregator.Services;
using ExpertSystem.Common;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Aggregator
{
    public class Program : ServerProgram
    {
        private readonly SocketExchange.SocketExchangeClient _client;

        private Program(ServerProgramOptions options) : base(options)
        {
            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            _client = new SocketExchange.SocketExchangeClient(channel);

            try
            {
                var message = _client.SayHello(new HelloMessage {Version = Options.Version});
                Console.WriteLine($"Connected to SocketExchange v{message.Version} on 127.0.0.1:50051");
            }
            catch
            {
                Console.WriteLine("Error: Connection to SocketExchange on 127.0.0.1:50051 failed");
                Environment.Exit(1);
            }
        }

        private async Task<Program> Init()
        {
            var socketCache = new EntityCache<CustomSocket>(new EntityCacheOptions {IdPropertyName = "SocketName"});
            var socketGroupCache = new EntityCache<SocketGroup>(new EntityCacheOptions {IdPropertyName = "GroupName"});

            var socketStream = _client.GetSockets(new Empty()).ResponseStream;
            while (await socketStream.MoveNext(CancellationToken.None))
                socketCache.Add(socketStream.Current);

            var socketGroupStream = _client.GetSocketGroups(new Empty()).ResponseStream;
            while (await socketGroupStream.MoveNext(CancellationToken.None))
                socketGroupCache.Add(socketGroupStream.Current);

            var operationsOptions = new SocketOperationsOptions {Debug = Options.Debug, Version = Options.Version};
            var socketOperations = new SocketOperationsImpl(socketCache, socketGroupCache, _client, operationsOptions);

            Server = new Grpc.Core.Server
            {
                Services = {SocketOperations.BindService(socketOperations)},
                Ports = {new ServerPort("localhost", Options.Port, ServerCredentials.Insecure)}
            };

            return this;
        }

        private static void Main()
        {
            var program = new Program(new ServerProgramOptions
                {Name = "SocketOperations", Version = "1.0.0", Debug = true, Port = 50052});
            program.Init().GetAwaiter().GetResult();
            program.Run();
        }
    }
}