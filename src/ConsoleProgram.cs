using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem
{
    public enum Command
    {
        Exit = 0, ForwardProcessing = 1, BackProcessing = 2, LogicProcessing = 3, FuzzyProcessingMamdani = 4, FuzzyProcessingSugeno = 5
    }

    public class ConsoleProgram : Program
    {
        public ConsoleProgram(ProgramOptions options) : base(options) { }

        public void Run()
        {
            string choice;
            PrintCommands();

            while ((choice = Console.ReadLine()) != ((int)Command.Exit).ToString())
            {
                string socketName;
                List<Fact> socketFacts;

                var choiceNum = (Command)int.Parse(choice);
                switch (choiceNum)
                {
                    case Command.BackProcessing:
                        Console.WriteLine("Обратный продукционный вывод, введите название разъема: ");
                        socketName = Console.ReadLine();
                        BackProcessing(socketName);
                        break;
                    case Command.ForwardProcessing:
                        Console.WriteLine("Прямой продукционный вывод, введите факты для поиска разъема: ");
                        socketFacts = GetSocketFactsFromConsole();
                        ForwardProcessing(new FactSet(socketFacts.ToArray()));
                        break;
                    case Command.LogicProcessing:
                        Console.WriteLine("Логический вывод, введите посылки: ");
                        socketFacts = GetSocketFactsFromConsole();
                        Console.WriteLine("Логический вывод, введите утверждение (НазваниеРазъема): ");
                        socketName = Console.ReadLine();
                        LogicProcessing(new FactSet(socketFacts.ToArray()), socketName);
                        break;
                    default:
                        Console.WriteLine("Команда не распознана");
                        break;
                }
                PrintCommands();
            }
        }

        private FactSet BackProcessing(string socketName)
        {
            WritePaddedBottom($"Обратный продукционный вывод для {socketName}");
            try
            {
                var socketFacts = _productionProcessor.BackProcessing(socketName);
                WritePaddedTop($"Результат для {socketName}: {socketFacts}");
                return socketFacts;
            }
            catch (Exception ex)
            {
                WritePaddedTop(ex.Message);
                return null;
            }
        }

        private List<string> ForwardProcessing(FactSet factSet)
        {
            WritePaddedBottom($"Прямой продукционный вывод для {factSet}");
            var socketList = _productionProcessor.ForwardProcessing(factSet);
            WritePaddedTop("Возможные разъемы: " + string.Join(", ", socketList));
            return socketList;
        }

        private bool LogicProcessing(FactSet factSet, string socketName)
        {
            WritePaddedBottom($"Логический вывод утверждения {socketName} при посылках {factSet}");
            var isCorrect = _logicProcessor.Processing(factSet, socketName);
            WritePaddedTop(isCorrect ? $"Утверждение для {socketName} верно" : "Утверждение неверно");
            return isCorrect;
        }

        private static void PrintCommands()
        {
            WritePaddedTop("Выберите действие: ");
            Console.WriteLine($"{(int)Command.ForwardProcessing} - прямой продукционный вывод");
            Console.WriteLine($"{(int)Command.BackProcessing} - обратный продукционный вывод");
            Console.WriteLine($"{(int)Command.LogicProcessing} - логический вывод");
            Console.WriteLine($"{(int)Command.FuzzyProcessingMamdani} - нечеткий вывод (Мамдани)");
            Console.WriteLine($"{(int)Command.FuzzyProcessingSugeno} - нечеткий вывод (Сугэно)");
            WritePaddedBottom($"{(int)Command.Exit} - выход");
        }

        private static void PrintDomains()
        {
            WritePaddedTop("Выберите домен: ");
            foreach (SocketDomain domain in GetSocketDomains())
            {
                var domainChoice = ((int)domain).ToString().PadRight(3);
                Console.WriteLine($"{domainChoice} - {GetSocketDomainName(domain)}");
            }
            Console.WriteLine($"{0.ToString().PadRight(3)} - завершить ввод и продолжить");
        }

        private static List<Fact> GetSocketFactsFromConsole()
        {
            WritePaddedTop("Добавьте следующее свойство: ");
            string domainChoice;
            PrintDomains();
            var factsList = new List<Fact>();
            while ((domainChoice = Console.ReadLine()) != (0).ToString())
            {
                var domain = (SocketDomain)int.Parse(domainChoice);
                if (Enum.IsDefined(typeof(SocketDomain), domain))
                {
                    if (factsList.Any(p => p.Domain.Equals(domain.ToString())))
                        Console.WriteLine($"Поле со свойством {domain.ToString()} уже существует");
                    else
                    {
                        WritePaddedTop("Введите значение");
                        var value = Console.ReadLine();
                        factsList.Add(new Fact(domain, value));
                    }
                }
                else
                    Console.WriteLine("Команда не распознана");

                PrintDomains();
            }

            return factsList;
        }

        private static void WritePaddedTop(string message)
        {
            Console.WriteLine("".PadLeft(40, '-'));
            Console.WriteLine(message);
        }

        private static void WritePaddedBottom(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("".PadLeft(40, '-'));
        }
    }
}