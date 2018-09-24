using System;
using ExpertSystem.Models;
using ExpertSystem.ProductionProccessor;

namespace ExpertSystem
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var socketFieldsProcessor = new SocketFieldsProcessor();
            var sockets = socketFieldsProcessor.GetSockets();
            var fieldValues = socketFieldsProcessor.GetFieldsWithPossibleValues(sockets);

            var rulesGenerator = new RulesGenerator();
            var rulesGraph = rulesGenerator.GenerateRules(sockets, fieldValues);
            var processor = new Processor(rulesGraph);

            // Пример поиска
            var facts = processor.BackProcessing("5145167-4");

            Console.ReadLine();
        }
    }
}