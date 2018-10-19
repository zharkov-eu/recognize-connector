using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExpertSystem.Client.Models;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Client
{
    public enum Command
    {
        Exit = 0,
        ForwardProcessing = 1,
        BackProcessing = 2,
        LogicProcessing = 3,
        FuzzyProcessingMamdani = 4,
        FuzzyProcessingSugeno = 5
    }

    public class ConsoleProgram : Program
    {
        public ConsoleProgram(ProgramOptions options)
            : base(options)
        {
        }

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
                        WritePaddedTop("Обратный продукционный вывод, введите название разъема:");
                        Console.Write("Название разъема: ");
                        socketName = Console.ReadLine();
                        BackProcessing(socketName);
                        break;
                    case Command.ForwardProcessing:
                        WritePaddedTop("Прямой продукционный вывод, введите факты:");
                        socketFacts = GetSocketFactsFromConsole();
                        ForwardProcessing(new FactSet(socketFacts.ToArray()));
                        break;
                    case Command.LogicProcessing:
                        WritePaddedTop("Логический вывод, введите посылки:");
                        socketFacts = GetSocketFactsFromConsole();
                        WritePaddedTop("Логический вывод, введите утверждение:");
                        Console.Write("Название разъема: ");
                        socketName = Console.ReadLine();
                        LogicProcessing(new FactSet(socketFacts.ToArray()), socketName);
                        break;
                    case Command.FuzzyProcessingMamdani:
                        WritePaddedTop("Нечеткий вывод Мамдани, введите факты:");
                        socketFacts = GetSocketFactsFromConsole(new[]
                        {
                            SocketDomain.NumberOfContacts, SocketDomain.SizeLength, SocketDomain.SizeWidth
                        });
                        FuzzyProcessingMamdani(new FactSet(socketFacts.ToArray()));
                        break;
                    case Command.FuzzyProcessingSugeno:
                        WritePaddedTop("Нечеткий вывод Сугэно, введите факты:");
                        socketFacts = GetSocketFactsFromConsole(new[]
                        {
                            SocketDomain.NumberOfContacts, SocketDomain.SizeLength, SocketDomain.SizeWidth
                        });
                        FuzzyProcessingSugeno(new FactSet(socketFacts.ToArray()));
                        break;
                    default:
                        WritePaddedTop("Команда не распознана");
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
                var socketFacts = ProductionProcessor.BackProcessing(socketName);
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
            var socketList = ProductionProcessor.ForwardProcessing(factSet);
            WritePaddedTop("Возможные разъемы: " + string.Join(", ", socketList));
            return socketList;
        }

        private bool LogicProcessing(FactSet factSet, string socketName)
        {
            WritePaddedBottom($"Логический вывод утверждения {socketName} при посылках {factSet}");
            var isCorrect = LogicProcessor.Processing(factSet, socketName);
            WritePaddedTop(isCorrect ? $"Утверждение для {socketName} верно" : "Утверждение неверно");
            return isCorrect;
        }

        private double FuzzyProcessingMamdani(FactSet factSet)
        {
            WritePaddedBottom(
                $"Нечеткий вывод (Мамдани) максимальной силы тока при разрыве цепи при известных {factSet}");
            var amperageCircuit = FuzzyProcessor.MamdaniProcesing(factSet);
            WritePaddedTop($"Максимальная сила тока при разрыве цепи: {amperageCircuit} мА");
            return amperageCircuit;
        }

        private double FuzzyProcessingSugeno(FactSet factSet)
        {
            WritePaddedBottom(
                $"Нечеткий вывод (Сугэно) максимальной силы тока при разрыве цепи при известных {factSet}");
            var amperageCircuit = FuzzyProcessor.SugenoProcesing(factSet);
            WritePaddedTop($"Максимальная сила тока при разрыве цепи: {amperageCircuit} мА");
            return amperageCircuit;
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

        private static void PrintDomains(IEnumerable<SocketDomain> domains)
        {
            if (domains == null)
                domains = GetSocketDomains();

            WritePaddedTop("Выберите домен: ");
            foreach (var domain in GetSocketDomains().Where(p => domains.Contains(p)))
            {
                var domainChoice = ((int)domain).ToString().PadRight(3);
                Console.WriteLine($"{domainChoice} - {GetSocketDomainName(domain)}");
            }

            Console.WriteLine($"{0.ToString().PadRight(3)} - завершить ввод и продолжить");
        }

        private static List<Fact> GetSocketFactsFromConsole(IEnumerable<SocketDomain> domains = null)
        {
            WritePaddedTop("Добавьте следующее свойство: ");
            string domainChoice;
            PrintDomains(domains);
            var factsList = new List<Fact>();
            while ((domainChoice = Console.ReadLine()) != 0.ToString())
            {
                var domain = (SocketDomain)int.Parse(domainChoice);
                if (Enum.IsDefined(typeof(SocketDomain), domain))
                    if (factsList.Any(p => p.Domain.Equals(domain.ToString())))
                    {
                        Console.WriteLine($"Поле со свойством {domain.ToString()} уже существует");
                    }
                    else
                    {
                        WritePaddedTop("Введите значение");

                        var type = SocketDomainType[domain];
                        var converter = TypeDescriptor.GetConverter(type);
                        Console.Write($"{GetSocketDomainName(domain).FirstCharToUpper()} ({type}): ");

                        var value = Console.ReadLine();
                        factsList.Add(new Fact(domain, converter.ConvertFrom(value)));
                    }
                else
                    Console.WriteLine("Команда не распознана");

                PrintDomains(domains);
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

    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}