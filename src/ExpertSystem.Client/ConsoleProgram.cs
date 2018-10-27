using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using Grpc.Core.Utils;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Client
{
    public enum Command
    {
        Exit,
        ForwardProcessing,
        BackProcessing,
        LogicProcessing,
        FuzzyProcessingMamdani,
        FuzzyProcessingSugeno,
        FuzzyNeuralProcessing,
        AddNewSocket,
        UpdateExistingSocket,
        DeleteExistingSocket
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
                var socketType = typeof(CustomSocket);

                var choiceNum = (Command)int.Parse(choice);
                switch (choiceNum)
                {
                    case Command.ForwardProcessing:
                        WritePaddedTop("Прямой продукционный вывод, введите факты:");
                        socketFacts = GetSocketFactsFromConsole();
                        ForwardProcessing(new FactSet(socketFacts.ToArray())).ConfigureAwait(false);
                        break;

                    case Command.BackProcessing:
                        WritePaddedTop("Обратный продукционный вывод, введите название разъема:");
                        Console.Write("Название разъема: ");
                        socketName = Console.ReadLine();
                        BackProcessing(socketName).ConfigureAwait(false);
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

                    case Command.FuzzyNeuralProcessing:
                        WritePaddedTop("Нейро-нечеткий вывод с использованием ANFIS, введите факты:");
                        socketFacts = GetSocketFactsFromConsole(new[]
                        {
                            SocketDomain.NumberOfContacts, SocketDomain.SizeLength, SocketDomain.SizeWidth
                        });
                        FuzzyNeuralProcessing(new FactSet(socketFacts.ToArray()));
                        break;

                    case Command.AddNewSocket:
                        WritePaddedTop("Добавление нового разъема, введите название:");
                        var newSocketName = Console.ReadLine();
                        //if (SocketCache.SocketExists(newSocketName))
                        //{
                        //    Console.WriteLine($"Разъем {newSocketName} уже существует");
                        //    break;
                        //}

                        WritePaddedTop("Добавление нового разъема, введите характеристики:");
                        var creatingSocket = new CustomSocket();
                        foreach (var fact in GetSocketFactsFromConsole())
                            socketType.GetProperty(fact.Domain.ToString()).SetValue(creatingSocket, fact.Value);
                        creatingSocket.SocketName = newSocketName;

                        Client.UpsertSocket(creatingSocket);
                        //SocketCache.Add(creatingSocket);

                        Console.WriteLine($"Разъем: {creatingSocket} был успешно добавлен.");
                        break;

                    case Command.UpdateExistingSocket:
                        WritePaddedTop("Обновление существующего разъема, введите название разъема:");
                        var updatingSocketName = Console.ReadLine();
                        //if (!SocketCache.SocketExists(updatingSocketName))
                        //{
                        //    Console.WriteLine($"Разъем {updatingSocketName} не существует");
                        //    break;
                        //}

                        //var updatingSocket = SocketCache.Get(updatingSocketName);
                        //Console.WriteLine($"Выбран разъем {updatingSocket}");

                        //WritePaddedTop("Обновление существующего разъема, введите характеристики:");
                        //foreach (var fact in GetSocketFactsFromConsole())
                        //    socketType.GetProperty(fact.Domain.ToString()).SetValue(updatingSocket, fact.Value);

                        //Client.UpsertSocket(updatingSocket);
                        //SocketCache.Update(updatingSocketName, updatingSocket);

                        //Console.WriteLine($"Разъем: {updatingSocket} был успешно обновлен.");
                        break;

                    case Command.DeleteExistingSocket:
                        WritePaddedTop("Удаление существующего разъема, введите название разъема:");
                        var deletingSocketName = Console.ReadLine();
                        //if (!SocketCache.SocketExists(deletingSocketName))
                        //{
                        //    Console.WriteLine($"Разъем {deletingSocketName} не существует");
                        //    break;
                        //}

                        //var deletingSocket = SocketCache.Get(deletingSocketName);
                        //Client.DeleteSocket(deletingSocket);
                        //SocketCache.Remove(deletingSocketName);

                        Console.WriteLine($"Разъем {deletingSocketName} был успешно удален.");
                        break;

                    default:
                        WritePaddedTop("Команда не распознана");
                        break;
                }

                PrintCommands();
            }
        }

        private async Task ForwardProcessing(FactSet factSet)
        {
            WritePaddedBottom($"Прямой продукционный вывод для {factSet}");
            var socketType = typeof(CustomSocket);
            var customSocket = new CustomSocket();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            var foundSockets = await Client.FindSocketsByParams(customSocket).ResponseStream.ToListAsync();
            WritePaddedTop("Возможные разъемы: " + string.Join(", ", foundSockets));
        }

        private async Task BackProcessing(string socketName)
        {
            WritePaddedBottom($"Обратный продукционный вывод для {socketName}");
            try
            {
                var socketIdentity = new CustomSocketIdentity
                {
                    SocketName = socketName
                };
                var socketFacts = await Client.FindSocketByIdentityAsync(socketIdentity);
                //TODO Женя, сам итерируйся по этому ебливому сокету, можно же было вернуть массив фактов
                WritePaddedTop($"Результат для {socketName}: {socketFacts}");
            }
            catch (Exception ex)
            {
                WritePaddedTop(ex.Message);
            }
        }

        private bool LogicProcessing(FactSet factSet, string socketName)
        {
            WritePaddedBottom($"Логический вывод утверждения {socketName} при посылках {factSet}");
            var socketType = typeof(CustomSocket);
            var customSocket = new CustomSocket
            {
                SocketName = socketName
            };
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            var fuckingBullshit = Client.IsParamsMatchSocket(customSocket);
            var isCorrect = fuckingBullshit != null;
            WritePaddedTop(isCorrect ? $"Утверждение для {socketName} верно" : "Утверждение неверно");
            return isCorrect;
        }

        private double FuzzyProcessingMamdani(FactSet factSet)
        {
            WritePaddedBottom($"Нечеткий вывод (Мамдани) максимальной силы тока при разрыве цепи при известных {factSet}");

            var socketType = typeof(CustomSocket);
            var customSocket = new FuzzySocketParams();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            var amperageCircuit = Client.FuzzyGetAmperageCircuitByParams(new FuzzySocketRequest
            {
                Method = FuzzyMethod.Mamdani,
                Socket = customSocket
            }).AmperageCircuit;

            WritePaddedTop($"Максимальная сила тока при разрыве цепи: {amperageCircuit} мА");
            return amperageCircuit;
        }

        private double FuzzyProcessingSugeno(FactSet factSet)
        {
            WritePaddedBottom($"Нечеткий вывод (Сугэно) максимальной силы тока при разрыве цепи при известных {factSet}");
            var socketType = typeof(CustomSocket);
            var customSocket = new FuzzySocketParams();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            var amperageCircuit = Client.FuzzyGetAmperageCircuitByParams(new FuzzySocketRequest
            {
                Method = FuzzyMethod.Sugeno,
                Socket = customSocket
            }).AmperageCircuit;

            WritePaddedTop($"Максимальная сила тока при разрыве цепи: {amperageCircuit} мА");
            return amperageCircuit;
        }

        private double FuzzyNeuralProcessing(FactSet factSet)
        {
            WritePaddedBottom(
                $"Нейро-нечеткий вывод (ANFIS) максимальной силы тока при разрыве цепи при известных {factSet}");

            var socketType = typeof(CustomSocket);
            var customSocket = new FuzzySocketParams();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            var amperageCircuit = Client.FuzzyGetAmperageCircuitByParams(new FuzzySocketRequest
            {
                Method = FuzzyMethod.Neural,
                Socket = customSocket
            }).AmperageCircuit;

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
            Console.WriteLine($"{(int)Command.FuzzyNeuralProcessing} - нейро-нечеткий вывод (ANFIS)");
            Console.WriteLine($"{(int)Command.AddNewSocket} - добавление нового разъёма");
            Console.WriteLine($"{(int)Command.UpdateExistingSocket} - обновление существующего разъёма");
            Console.WriteLine($"{(int)Command.DeleteExistingSocket} - удаление существующего разъёма");
            WritePaddedBottom($"{(int)Command.Exit} - выход");
        }

        private static void PrintDomains(IEnumerable<SocketDomain> domains = null)
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
                    if (factsList.Any(p => p.Domain.Equals(domain)))
                        Console.WriteLine($"Поле со свойством {domain.ToString()} уже существует");
                    else
                    {
                        WritePaddedTop("Введите значение");

                        var type = SocketDomainType[domain];
                        var converter = TypeDescriptor.GetConverter(type);
                        Console.Write($"{GetSocketDomainName(domain).FirstCharToUpper()}: ");

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