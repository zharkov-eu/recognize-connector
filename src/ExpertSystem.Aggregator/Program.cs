using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using ExpertSystem.Aggregator.Processors;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Aggregator.Services;
using ExpertSystem.Client.Services;
using ExpertSystem.Common;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Aggregator
{
    public class Program : ServerProgram
    {
        private readonly SocketExchange.SocketExchangeClient _client;
        private readonly SocketCache _socketCache;

        private Program(ServerProgramOptions options) : base(options)
        {
            _socketCache = new SocketCache();

            var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            _client = new SocketExchange.SocketExchangeClient(channel);

            try
            {
                var message = _client.SayHello(new HelloMessage {Version = Options.Version});
                Console.WriteLine($"Connected to SocketExchange v{message.Version} on 127.0.0.1:50051");
            }
            catch
            {
                Console.WriteLine("Error: Connection to SocketExchange on 127.0.0.1:50051 failed");
                Environment.Exit(1);
            }
        }

        private async Task<Program> Init()
        {
            var stream = _client.GetSockets(new Empty()).ResponseStream;
            while (await stream.MoveNext(CancellationToken.None))
                _socketCache.Add(stream.Current);

            var rulesGenerator = new ProductionRulesGenerator();
            var logicRulesGenerator = new LogicRulesGenerator();
            var fuzzyRulesGenerator = new FuzzyRulesGenerator();
            var neuralRulesGenerator = new NeuralRulesGenerator();

            // Продукционный вывод
            var rulesGraph = rulesGenerator.GenerateRules(_socketCache.GetAll());
            // Логический вывод
            var logicRules = logicRulesGenerator.GenerateRules(_socketCache.GetAll());
            // Нечеткий вывод
            var fuzzyDomains = fuzzyRulesGenerator.GetFuzzyDomains(_socketCache.GetAll());
            var fuzzyFacts = fuzzyRulesGenerator.GetFuzzyFacts(fuzzyDomains, _socketCache.GetAll());
            // Нейро-нечеткий вывод
            var neuralNetwork = neuralRulesGenerator.GetNeuralNetwork(_socketCache.GetAll());

            Server = new Grpc.Core.Server
            {
                Services =
                {
                    SocketOperations.BindService(new SocketOperationsImpl(
                        new ProductionProcessor(rulesGraph, new ProcessorOptions {Debug = Options.Debug}),
                        new LogicProcessor(logicRules, new ProcessorOptions {Debug = Options.Debug}),
                        new FuzzyProcessor(fuzzyDomains, fuzzyFacts, new ProcessorOptions {Debug = Options.Debug}),
                        new NeuralProcessor(neuralNetwork, new ProcessorOptions {Debug = Options.Debug})
                    ))
                },
                Ports = {new ServerPort("localhost", Options.Port, ServerCredentials.Insecure)}
            };

            return this;
        }

        private static void Main()
        {
            var program = new Program(new ServerProgramOptions
                {Name = "SocketOperations", Version = "1.0.0", Debug = true, Port = 50052});
            program.Init().GetAwaiter().GetResult();
            program.Run();
        }
    }
}