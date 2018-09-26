using System;
using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Processor;

namespace ExpertSystem
{
    public enum Command {
        Exit = 0, ForwardProcessing = 1, BackProcessing = 2, LogicProcessing = 3, FuzzyProcessing = 4
    };

    public class ConsoleProgram : Program
    {
        public ConsoleProgram(ProgramOptions options) : base(options) {}

        public void Run()
        {
            string choice;
            PrintCommands();
            while ((choice = Console.ReadLine()) != ((int)Command.Exit).ToString())
            {
                Command choiceNum = (Command) Int32.Parse(choice);
                switch (choiceNum)
                {
                    case Command.BackProcessing:
                        Console.WriteLine("Обратный продукционный вывод, введите название разъема: ");
                        string socketName = Console.ReadLine();
                        BackProcessing(socketName);
                        break;
                    default:
                        Console.WriteLine("Комманда не распознана");
                        break;
                }
                PrintCommands();
            }
        }

        private FactSet BackProcessing(string socketName)
        {
            WritePaddedBottom($"Обратный продукционный вывод для {socketName}");
            try {
                var socketFacts = _productionProcessor.BackProcessing(socketName);
                WritePaddedTop($"Результат для {socketName}: {socketFacts.ToString()}");
                return socketFacts;
            } catch (Exception ex) {
                WritePaddedTop(ex.Message);
                return null;
            }
        }

        private List<string> ForwardProcessing(FactSet factSet)
        {
            WritePaddedBottom($"Прямой продукционный вывод для {factSet.ToString()}");
            var socketList = _productionProcessor.ForwardProcessing(factSet);
            WritePaddedTop("Возможные разъемы: " + string.Join(", ", socketList));
            return socketList;
        }

        private void PrintCommands()
        {
            WritePaddedTop("Выберите действие: ");
            Console.WriteLine($"{(int)Command.ForwardProcessing} - прямой продукционный вывод");
            Console.WriteLine($"{(int)Command.BackProcessing} - обратный продукционный вывод");
            Console.WriteLine($"{(int)Command.LogicProcessing} - логический вывод");
            Console.WriteLine($"{(int)Command.FuzzyProcessing} - нечеткий вывод");
            WritePaddedBottom($"{(int)Command.Exit} - выход");
        }

        private void WritePaddedTop(string message)
        {
             Console.WriteLine("".PadLeft(40, '-'));
             Console.WriteLine(message);
        }

        private void WritePaddedBottom(string message)
        {
             Console.WriteLine(message);
             Console.WriteLine("".PadLeft(40, '-'));
        }
    }
}