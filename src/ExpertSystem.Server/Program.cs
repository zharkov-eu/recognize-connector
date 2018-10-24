using System.IO;
using System.Runtime.CompilerServices;
using ExpertSystem.Common;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Parsers;
using ExpertSystem.Server.DAL.Controllers;
using ExpertSystem.Server.DAL.Extensions;
using ExpertSystem.Server.DAL.Repositories;
using ExpertSystem.Server.Services;
using Grpc.Core;

namespace ExpertSystem.Server
{
	public class Program : ServerProgram
	{
		private Program(ServerProgramOptions options) : base(options)
		{
			var dataPath = Path.Combine(GetThisFileDirectory(), "..", "..", "data");

			var socketOptions = new CsvRepositoryOptions
			{
				IdPropertyName = "SocketName",
				CsvFileName = Path.Combine(dataPath, "1.csv"),
				WalFileName = Path.Combine(dataPath, "wal.txt")
			};
			var socketExtenstion = new ServerSocketExtension();
			var socketsRepository = new CsvRepository<CustomSocket>(socketOptions, socketExtenstion,
					new CustomParser<CustomSocket>(socketExtenstion)).Sync();
			var socketController = new SocketController(socketsRepository);

			var categoryOptions = new CsvRepositoryOptions
			{
				IdPropertyName = "SocketName",
				CsvFileName = Path.Combine(dataPath, "categoryData.csv"),
				WalFileName = Path.Combine(dataPath, "categoryWal.txt")
			};
			var categoryExtenstion = new ServerCategoryExtension();
			var categoryRepository = new CsvRepository<SocketGroup>(categoryOptions, categoryExtenstion,
				new CustomParser<SocketGroup>(categoryExtenstion)).Sync();
			var categoryController = new CategoryController(categoryRepository);

			Server = new Grpc.Core.Server
			{
				Services =
				{
					SocketExchange.BindService(
						new SocketExchangeImpl(socketController, categoryController,
							new SocketExchangeOptions {Version = Options.Version, Debug = options.Debug})
					)
				},
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