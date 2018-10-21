using System;
using System.IO;
using Grpc.Core;
using ExpertSystem.Server.Services;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Repositories;

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

            var csvFileName = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "data", "1.csv");
            var walFileName = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "data", "wal.txt");
            var socketsRepository = new CsvRepository(csvFileName, walFileName).Sync();

            Server = new Grpc.Core.Server
            {
                Services = { SocketExchange.BindService(new SocketExchangeImpl(socketsRepository)) },
                Ports = { new ServerPort("localhost", Options.Port, ServerCredentials.Insecure) }
            };
        }

        private void Run()
        {
            Server.Start();
            Console.WriteLine("SocketExchange server listening on port " + Options.Port);

            Console.WriteLine("Press key 'q' to stop the server...");
            CheckShutdown();
        }

        private void CheckShutdown()
        {
            ConsoleKey key;
            if ((key = Console.ReadKey().Key) == ConsoleKey.Q)
                Server.ShutdownAsync().GetAwaiter().GetResult();
            else
            {
                Console.WriteLine($"\nCommand '{key.ToString()}' not recognized");
                CheckShutdown();
            }
        }

        private static void Main()
        {
            var program = new Program(new ProgramOptions { Debug = true, Port = 50051 });
            program.Run();
        }
    }
}