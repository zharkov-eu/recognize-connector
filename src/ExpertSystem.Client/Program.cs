using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using ExpertSystem.Client.Processors;
using ExpertSystem.Client.RulesGenerators;
using ExpertSystem.Client.Services;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Client
{
    public class Program
    {
        protected readonly SocketExchange.SocketExchangeClient Client;
        protected readonly SocketCache SocketCache;
        protected readonly ProgramOptions Options;
        protected ProductionProcessor ProductionProcessor;
        protected LogicProcessor LogicProcessor;
        protected FuzzyProcessor FuzzyProcessor;
        protected NeuralProcessor NeuralProcessor;

        public struct ProgramOptions
        {
            public bool Debug;
        }

        protected Program(ProgramOptions options)
        {
            Options = options;
            SocketCache = new SocketCache();

            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            Client = new SocketExchange.SocketExchangeClient(channel);
        }

        private async Task<Program> Init()
        {
            var stream = Client.GetSockets(new Empty()).ResponseStream;
            while (await stream.MoveNext())
                SocketCache.Add(stream.Current);

            var rulesGenerator = new ProductionRulesGenerator();
            var logicRulesGenerator = new LogicRulesGenerator();
            var fuzzyRulesGenerator = new FuzzyRulesGenerator();
            var neuralRulesGenerator = new NeuralRulesGenerator();

            // Продукционный вывод
            var rulesGraph = rulesGenerator.GenerateRules(SocketCache.GetAll());
            // Логический вывод
            var logicRules = logicRulesGenerator.GenerateRules(SocketCache.GetAll());
            // Нечеткий вывод
            var fuzzyDomains = fuzzyRulesGenerator.GetFuzzyDomains(SocketCache.GetAll());
            var fuzzyFacts = fuzzyRulesGenerator.GetFuzzyFacts(fuzzyDomains, SocketCache.GetAll());
            // Нейро-нечеткий вывод
            var neuralNetwork = neuralRulesGenerator.GetNeuralNetwork(SocketCache.GetAll());

            ProductionProcessor = new ProductionProcessor(rulesGraph, new ProcessorOptions {Debug = Options.Debug});
            LogicProcessor = new LogicProcessor(logicRules, new ProcessorOptions {Debug = Options.Debug});
            FuzzyProcessor = new FuzzyProcessor(fuzzyDomains, fuzzyFacts, new ProcessorOptions {Debug = Options.Debug});
            NeuralProcessor = new NeuralProcessor(neuralNetwork, new ProcessorOptions {Debug = Options.Debug});

            return this;
        }

        private static void Main()
        {
            var consoleProgram = new ConsoleProgram(new ProgramOptions {Debug = true});
            consoleProgram.Init().GetAwaiter().GetResult();
            consoleProgram.Run();
        }
    }
}