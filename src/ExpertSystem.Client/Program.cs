using System;
using Grpc.Core;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Client
{
    public class Program
    {
        private const string Version = "1.0.0";
        protected readonly SocketOperations.SocketOperationsClient Client;
        protected readonly ProgramOptions Options;

        public struct ProgramOptions
        {
            public bool Debug;
        }

        protected Program(ProgramOptions options)
        {
            Options = options;

            var channel = new Channel("127.0.0.1:50052", ChannelCredentials.Insecure);
            Client = new SocketOperations.SocketOperationsClient(channel);

            try
            {
                var message = Client.SayHello(new HelloMessage { Version = Version });
                Console.WriteLine($"Connected to SocketOperations v{message.Version} on 127.0.0.1:50052");
            }
            catch
            {
                Console.WriteLine("Error: Connection to SocketOperations on 127.0.0.1:50052 failed");
                Environment.Exit(1);
            }
        }

        private static void Main()
        {
            var consoleProgram = new ConsoleProgram(new ProgramOptions { Debug = true });
            consoleProgram.Run();
        }
    }
}