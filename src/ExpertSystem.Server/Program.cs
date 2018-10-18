using System.IO;
using System.Collections.Generic;
using ExpertSystem.Common.Models;
using ExpertSystem.Server.Parsers;

namespace ExpertSystem.Server
{
    public class Program
    {
        public struct ProgramOptions
        {
            public bool Debug;
        }

        protected Program(ProgramOptions options)
        {
            List<CustomSocket> sockets;
            var socketFieldsProcessor = new SocketFieldsProcessor();

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "..", "data", "1.csv");
            using (var stream = File.OpenRead(fileName))
                sockets = socketFieldsProcessor.GetSockets(stream);
        }

        public void Run()
        {}

        private static void Main()
        {
            var program = new Program(new ProgramOptions { Debug = true });
            program.Run();
        }
    }
}