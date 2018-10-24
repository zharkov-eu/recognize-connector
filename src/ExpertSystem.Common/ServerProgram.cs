using System;

namespace ExpertSystem.Common
{
    public struct ServerProgramOptions
    {
        public string Version;
        public string Name;
        public bool Debug;
        public int Port;
    }

    public abstract class ServerProgram
    {
        protected Grpc.Core.Server Server;
        protected readonly ServerProgramOptions Options;

        protected ServerProgram(ServerProgramOptions options)
        {
            Options = options;
        }

        protected void Run()
        {
            Server.Start();
            Console.WriteLine($"{Options.Name} v{Options.Version} server listening on port " + Options.Port);
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
    }
}