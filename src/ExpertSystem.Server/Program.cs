using System.IO;
using System.Collections.Generic;
using ExpertSystem.Common.RulesGenerators;
using ExpertSystem.Models;

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
            var fieldValues = socketFieldsProcessor.GetDomainsWithPossibleValues(sockets);

            var rulesGenerator = new ProductionRulesGenerator();
            var logicRulesGenerator = new LogicRulesGenerator();
            var fuzzyRulesGenerator = new FuzzyRulesGenerator();

            // Продукционный вывод
            var rulesGraph = rulesGenerator.GenerateRules(sockets, fieldValues);
            // Логический вывод
            var logicRules = logicRulesGenerator.GenerateRules(sockets);
            // Нечеткий вывод
            var fuzzyDomains = fuzzyRulesGenerator.GetFuzzyDomains(sockets);
            var fuzzyFacts = fuzzyRulesGenerator.GetFuzzyFacts(fuzzyDomains, sockets);
        }

        private static void Main()
        {
            var program = new ConsoleProgram(new ProgramOptions { Debug = true });
            program.Run();
        }
    }
}