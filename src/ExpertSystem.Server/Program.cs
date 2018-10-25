using System.IO;
using System.Runtime.CompilerServices;
using Grpc.Core;
using ExpertSystem.Common;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Parsers;
using ExpertSystem.Server.DAL.Controllers;
using ExpertSystem.Server.DAL.Repositories;
using ExpertSystem.Server.DAL.Serializers;
using ExpertSystem.Server.Services;

namespace ExpertSystem.Server
{
    public class Program : ServerProgram
    {
        private Program(ServerProgramOptions options) : base(options)
        {
            var dataPath = Path.Combine(GetThisFileDirectory(), "..", "..", "data");

            var socketSerializer = new CustomSocketSerializer();
            var socketParser = new CsvRecordParser<CustomSocket>(socketSerializer);
            var socketOptions = new CsvRepositoryOptions
            {
                IdPropertyName = "SocketName",
                CsvFileName = Path.Combine(dataPath, "1.csv"),
                WalFileName = Path.Combine(dataPath, "wal.txt")
            };
            var socketsRepository =
                new CsvRepository<CustomSocket>(socketSerializer, socketParser, socketOptions).Sync();
            var socketController = new CustomSocketController(socketsRepository);

            var socketGroupSerializer = new SocketGroupSerializer();
            var socketGroupParser = new CsvRecordParser<SocketGroup>(socketGroupSerializer);
            var socketGroupOptions = new CsvRepositoryOptions
            {
                IdPropertyName = "SocketName",
                CsvFileName = Path.Combine(dataPath, "categoryData.csv"),
                WalFileName = Path.Combine(dataPath, "categoryWal.txt")
            };
            var socketGroupRepository =
                new CsvRepository<SocketGroup>(socketGroupSerializer, socketGroupParser, socketGroupOptions).Sync();
            var socketGroupController = new SocketGroupController(socketGroupRepository);

            var socketExchangeOptions = new SocketExchangeOptions {Version = Options.Version, Debug = options.Debug};
            var socketExchange = new SocketExchangeImpl(socketController, socketGroupController, socketExchangeOptions);

            Server = new Grpc.Core.Server
            {
                Services = {SocketExchange.BindService(socketExchange)},
                Ports = {new ServerPort("localhost", Options.Port, ServerCredentials.Insecure)}
            };
        }

        private static void Main()
        {
            var program = new Program(
                new ServerProgramOptions {Name = "SocketExchange", Version = "1.0.0", Debug = true, Port = 50051}
            );
            program.Run();
        }

        private static string GetThisFileDirectory([CallerFilePath] string path = null)
        {
            return Path.GetDirectoryName(path);
        }
    }
}