using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using ExpertSystem.Client.Processors;
using ExpertSystem.Client.RulesGenerators;
using ExpertSystem.Common.Generated;
using Google.Protobuf.WellKnownTypes;

namespace ExpertSystem.Client
{
    public class Program
    {
        protected readonly SocketExchange.SocketExchangeClient Client;
        protected readonly ProgramOptions Options;
        protected ProductionProcessor ProductionProcessor;
        protected LogicProcessor LogicProcessor;
        protected FuzzyProcessor FuzzyProcessor;

        public struct ProgramOptions
        {
            public bool Debug;
        }

        protected Program(ProgramOptions options)
        {
            Options = options;

            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            Client = new SocketExchange.SocketExchangeClient(channel);
        }

        public async Task<Program> Init()
        {
            var sockets = new List<CustomSocket>();
            var stream = Client.GetSockets(new ExpertSystem.Common.Generated.Empty()).ResponseStream;
            while (await stream.MoveNext())
                sockets.Add(stream.Current);

            var rulesGenerator = new ProductionRulesGenerator();
            var logicRulesGenerator = new LogicRulesGenerator();
            var fuzzyRulesGenerator = new FuzzyRulesGenerator();

            // Продукционный вывод
            var rulesGraph = rulesGenerator.GenerateRules(sockets);
            // Логический вывод
            var logicRules = logicRulesGenerator.GenerateRules(sockets);
            // Нечеткий вывод
            var fuzzyDomains = fuzzyRulesGenerator.GetFuzzyDomains(sockets);
            var fuzzyFacts = fuzzyRulesGenerator.GetFuzzyFacts(fuzzyDomains, sockets);

            ProductionProcessor = new ProductionProcessor(rulesGraph, new ProcessorOptions { Debug = Options.Debug });
            LogicProcessor = new LogicProcessor(logicRules, new ProcessorOptions { Debug = Options.Debug });
            FuzzyProcessor = new FuzzyProcessor(fuzzyDomains, fuzzyFacts, new ProcessorOptions { Debug = Options.Debug });

            return this;
        }

        private static void Main()
        {
            var consoleProgram = new ConsoleProgram(new ProgramOptions { Debug = true });
            consoleProgram.Init().GetAwaiter().GetResult();
            consoleProgram.Run();
        }
    }
}