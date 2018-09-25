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
            var processor = new Processor(rulesGraph, new ProcessorOptions{ Debug = true });

            // Пример обратного продукционного вывода
            string socketName = "5145167-4";
            Console.WriteLine($"Обратный продукционный вывод для {socketName}");
            try {
                var socketFacts = processor.BackProcessing(socketName);
                Console.WriteLine($"Результат для {socketName}: {socketFacts.ToString()}");
            } catch (SystemException ex) {
                Console.WriteLine(ex.Message);
            }

            // Пример прямого продукционного вывода
            var numberOfPositions = new Fact("NumberOfPositions", "60");
            var numberOfContacts = new Fact("NumberOfContacts", "120");
            var gender = new Fact("Gender", "Female");
            FactSet factSet = new FactSet(numberOfPositions, numberOfContacts, gender);

            Console.WriteLine($"Прямой продукционный вывод для {factSet.ToString()}");
            var socketList = processor.ForwardProcessing(factSet);
            Console.WriteLine("Возможные разъемы: " + string.Join(", ", socketList));

            Console.ReadLine();
        }
    }
}