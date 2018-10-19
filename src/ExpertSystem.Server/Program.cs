using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using ExpertSystem.Server.Parsers;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Server
{
    public class Program
    {
        protected readonly Grpc.Core.Server Server;
        protected readonly ProgramOptions Options;

        public struct ProgramOptions
        {
            public bool Debug;
            public int Port;
        }

        protected Program(ProgramOptions options)
        {
            Options = options;

            List<CustomSocket> sockets;
            var socketFieldsProcessor = new SocketFieldsProcessor();

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "data", "1.csv");
            using (var stream = File.OpenRead(fileName))
                sockets = socketFieldsProcessor.GetSockets(stream);

            Server = new Grpc.Core.Server
            {
                Services = { SocketExchange.BindService(new SocketExchangeImpl(sockets)) },
                Ports = { new ServerPort("localhost", Options.Port, ServerCredentials.Insecure) }
            };
        }

        public async Task Run()
        {
            Server.Start();

            Console.WriteLine("SocketExchange server listening on port " + Options.Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            await Server.ShutdownAsync();
        }
            
        private static void Main()
        {
            var program = new Program(new ProgramOptions { Debug = true, Port = 50051 });
            program.Run().GetAwaiter().GetResult();
        }
    }
}