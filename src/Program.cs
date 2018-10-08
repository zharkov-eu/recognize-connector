using System.IO;
using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Processor;
using ExpertSystem.Processor.FuzzyProcessor;
using ExpertSystem.Processor.LogicProcessor;
using ExpertSystem.Processor.ProductionProcessor;

namespace ExpertSystem
{
    public class Program
    {
        protected readonly ProductionProcessor _productionProcessor;
        protected readonly LogicProcessor _logicProcessor;
        protected readonly FuzzyProcessor _fuzzyProcessor;

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
            var fieldValues = socketFieldsProcessor.GetDomainsWithPossibleValues(sockets);

            var rulesGenerator = new RulesGenerator();
            var logicRulesGenerator = new LogicRulesGenerator();
            var fuzzyRulesGenerator = new FuzzyRulesGenerator();

            var rulesGraph = rulesGenerator.GenerateRules(sockets, fieldValues);
            var logicRules = logicRulesGenerator.GenerateRules(sockets);
            var domainFacts = fuzzyRulesGenerator.GetFuzzyFacts(
                fuzzyRulesGenerator.GetFuzzyDomains(sockets), sockets
            );

            _productionProcessor = new ProductionProcessor(rulesGraph, new ProcessorOptions { Debug = options.Debug });
            _logicProcessor = new LogicProcessor(logicRules, new ProcessorOptions { Debug = options.Debug });
            _fuzzyProcessor = new FuzzyProcessor(domainFacts, new ProcessorOptions { Debug = options.Debug });
        }

        private static void Main(string[] args)
        {
            var program = new ConsoleProgram(new ProgramOptions { Debug = true });
            program.Run();
        }
    }
}