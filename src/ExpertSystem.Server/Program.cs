using System;
using System.IO;
using System.Runtime.CompilerServices;
using Grpc.Core;
using ExpertSystem.Server.Services;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using ExpertSystem.Common.Parsers;
using ExpertSystem.Server.DAL.Controllers;
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

            var basePath = Path.Combine(GetThisFileDirectory(), "..", "..", "data");
            
            var socketCsvFileName = Path.Combine(basePath, "1.csv");
            var socketWalFileName = Path.Combine(basePath, "wal.txt");
            var socketRepository = new CsvRepository<CustomSocket, CustomParser<CustomSocket, CustomSocketExtension>, CustomSocketExtension>(socketCsvFileName, socketWalFileName).Sync();
            var socketController = new SocketController(socketRepository);
            
            var categoryCsvFileName = Path.Combine(basePath, "category.csv");
            var categoryWalFileName = Path.Combine(basePath, "categoryWal.txt");
            var categoryRepository = new CsvRepository<Category, CustomParser, CategoryExtension>(categoryCsvFileName, categoryWalFileName);
            var categoryController = new CategoryController(categoryRepository);

            Server = new Grpc.Core.Server
            {
                Services = {SocketExchange.BindService(new SocketExchangeImpl(socketController, categoryController))},
                Ports = {new ServerPort("localhost", Options.Port, ServerCredentials.Insecure)}
            };
        }

        private void Run()
        {
            Server.Start();
            Console.WriteLine("SocketExchange server listening on port " + Options.Port);

            Console.WriteLine("Press key 'Q' to stop the server...");
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
            var program = new Program(new ProgramOptions {Debug = true, Port = 50051});
            program.Run();
        }
        
        private static string GetThisFileDirectory([CallerFilePath] string path = null)
        {
            return Path.GetDirectoryName(path);
        }
    }
}