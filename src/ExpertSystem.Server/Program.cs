using System.IO;
using System.Runtime.CompilerServices;
using Grpc.Core;
using ExpertSystem.Common;
using ExpertSystem.Server.Services;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Repositories;

namespace ExpertSystem.Server
{
    public class Program : ServerProgram
    {
        private Program(ServerProgramOptions options) : base(options)
        {
            public bool Debug;
            public int Port;
        }

        protected Program(ProgramOptions options)
        {
            Options = options;

            var csvFileName = Path.Combine(GetThisFileDirectory(), "..", "..", "data", "1.csv");
            var walFileName = Path.Combine(GetThisFileDirectory(), "..", "..", "data", "wal.txt");
            var socketsRepository = new CsvRepository(csvFileName, walFileName).Sync();

            Server = new Grpc.Core.Server
            {
                Services = {SocketExchange.BindService(new SocketExchangeImpl(socketsRepository))},
                Ports = {new ServerPort("localhost", Options.Port, ServerCredentials.Insecure)}
            };
        }

        private static void Main()
        {
            var program = new Program(new ServerProgramOptions
                {Name = "SocketExchange", Version = "1.0.0", Debug = true, Port = 50051});
            program.Run();
        }

        private static string GetThisFileDirectory([CallerFilePath] string path = null)
        {
            return Path.GetDirectoryName(path);
        }
    }
}