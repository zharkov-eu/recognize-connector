using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Models;

namespace ExpertSystem
{
    public enum Command
    {
        Exit = 0, ForwardProcessing = 1, BackProcessing = 2, LogicProcessing = 3, FuzzyProcessing = 4
    }

    public enum SocketDomain
    {
        Exit,
        Gender,
        ContactMaterial,
        ContactPlating,
        Color,
        HousingColor,
        HousingMaterial,
        MountingStyle,
        NumberOfContacts,
        NumberOfPositions,
        NumberOfRows,
        Orientation,
        PinPitch,
        Material,
        SizeDiameter,
        SizeLength,
        SizeHeight,
        SizeWidth
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
                var choiceNum = (Command)int.Parse(choice);
                switch (choiceNum)
                {
                    case Command.BackProcessing:
                        Console.WriteLine("Обратный продукционный вывод, введите название разъема: ");
                        var socketName = Console.ReadLine();
                        BackProcessing(socketName);
                        break;
                    case Command.ForwardProcessing:
                        Console.WriteLine("Прямой продукционный вывод, введите факты для поиска разъема: ");
                        var socketConsoleFacts = GetSocketFactsFromConsole();
                        ForwardProcessing(new FactSet(socketConsoleFacts.ToArray()));
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

        private static void PrintCommands()
        {
            WritePaddedTop("Выберите действие: ");
            Console.WriteLine($"{(int)Command.ForwardProcessing} - прямой продукционный вывод");
            Console.WriteLine($"{(int)Command.BackProcessing} - обратный продукционный вывод");
            Console.WriteLine($"{(int)Command.LogicProcessing} - логический вывод");
            Console.WriteLine($"{(int)Command.FuzzyProcessing} - нечеткий вывод");
            WritePaddedBottom($"{(int)Command.Exit} - выход");
        }

        private static void PrintDomains()
        {
            WritePaddedTop("Выберите домен: ");
            Console.WriteLine($"{(int)SocketDomain.Gender} - пол разъема");
            Console.WriteLine($"{(int)SocketDomain.ContactMaterial} - контактный материал");
            Console.WriteLine($"{(int)SocketDomain.ContactPlating} - контактная плата");
            Console.WriteLine($"{(int)SocketDomain.ContactMaterial} - контактный материал");
            Console.WriteLine($"{(int)SocketDomain.Color} - цвет");
            Console.WriteLine($"{(int)SocketDomain.HousingColor} - цвет корпуса)))");
            Console.WriteLine($"{(int)SocketDomain.HousingMaterial} - материал корпуса)))");
            Console.WriteLine($"{(int)SocketDomain.MountingStyle} - тип установки");
            Console.WriteLine($"{(int)SocketDomain.NumberOfContacts} - число контактов");
            Console.WriteLine($"{(int)SocketDomain.NumberOfPositions} - число позиций");
            Console.WriteLine($"{(int)SocketDomain.NumberOfRows} - число строк");
            Console.WriteLine($"{(int)SocketDomain.Orientation} - положение разъема");
            Console.WriteLine($"{(int)SocketDomain.PinPitch} - Шаг между контактами");
            Console.WriteLine($"{(int)SocketDomain.Material} - материал разъема");
            Console.WriteLine($"{(int)SocketDomain.SizeDiameter} - диаметр разъема");
            Console.WriteLine($"{(int)SocketDomain.SizeLength} - длина разъема");
            Console.WriteLine($"{(int)SocketDomain.SizeHeight} - высота разъема");
            Console.WriteLine($"{(int)SocketDomain.SizeWidth} - ширина разъема");
            WritePaddedBottom($"{(int)SocketDomain.Exit} - выход");
        }

        private static List<Fact> GetSocketFactsFromConsole()
        {
            WritePaddedTop("Добавьте следующее свойство: ");
            string domenChoice;
            PrintDomains();
            var factsList = new List<Fact>();
            while ((domenChoice = Console.ReadLine()) != ((int)SocketDomain.Exit).ToString())
            {
                var choiceNum = (SocketDomain)int.Parse(domenChoice);
                if (Enum.IsDefined(typeof(SocketDomain), choiceNum))
                {
                    if (factsList.Any(p => p.Domain.Equals(choiceNum.ToString())))
                        Console.WriteLine($"Поле со свойством {choiceNum.ToString()} уже существует");
                    else
                    {
                        WritePaddedTop("Введите значение");
                        var value = Console.ReadLine();
                        factsList.Add(new Fact(choiceNum.ToString(), value));
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