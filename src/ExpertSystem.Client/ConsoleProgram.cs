using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ExpertSystem.Client.Models;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using Grpc.Core.Utils;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Client
{
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

            while ((choice = Console.ReadLine()) != ((int)ConsoleCommands.Exit).ToString())
            {
                string socketName;
                List<Fact> socketFacts;

                var choiceNum = (ConsoleCommands)int.Parse(choice);
                switch (choiceNum)
                {
                    case ConsoleCommands.ForwardProcessing:
                        WritePaddedTop("Прямой продукционный вывод, введите факты:");
                        socketFacts = GetSocketFactsFromConsole();
                        ForwardProcessing(new FactSet(socketFacts.ToArray())).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.BackProcessing:
                        WritePaddedTop("Обратный продукционный вывод, введите название разъема:");
                        Console.Write("Название разъема: ");
                        socketName = Console.ReadLine();
                        BackProcessing(socketName).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.LogicProcessing:
                        WritePaddedTop("Логический вывод, введите посылки:");
                        socketFacts = GetSocketFactsFromConsole();
                        WritePaddedTop("Логический вывод, введите утверждение:");
                        Console.Write("Название разъема: ");
                        socketName = Console.ReadLine();
                        LogicProcessing(new FactSet(socketFacts.ToArray()), socketName).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.FuzzyProcessingMamdani:
                        WritePaddedTop("Нечеткий вывод Мамдани, введите факты:");
                        socketFacts = GetSocketFactsFromConsole(new List<SocketDomain>
                        {
                            SocketDomain.NumberOfContacts, SocketDomain.SizeLength, SocketDomain.SizeWidth
                        });
                        FuzzyProcessingMamdani(new FactSet(socketFacts.ToArray())).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.FuzzyProcessingSugeno:
                        WritePaddedTop("Нечеткий вывод Сугэно, введите факты:");
                        socketFacts = GetSocketFactsFromConsole(new List<SocketDomain>
                        {
                            SocketDomain.NumberOfContacts, SocketDomain.SizeLength, SocketDomain.SizeWidth
                        });
                        FuzzyProcessingSugeno(new FactSet(socketFacts.ToArray())).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.FuzzyNeuralProcessing:
                        WritePaddedTop("Нейро-нечеткий вывод с использованием ANFIS, введите факты:");
                        socketFacts = GetSocketFactsFromConsole(new List<SocketDomain>
                        {
                            SocketDomain.NumberOfContacts, SocketDomain.SizeLength, SocketDomain.SizeWidth
                        });
                        FuzzyNeuralProcessing(new FactSet(socketFacts.ToArray())).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.AddNewSocket:
                        WritePaddedTop("Добавление нового разъема, введите название:");
                        var newSocketName = Console.ReadLine();
                        if (GetSocketByName(newSocketName).Result != null)
                        {
                            Console.WriteLine($"Разъем {newSocketName} уже существует");
                            break;
                        }

                        WritePaddedTop("Добавление нового разъема, введите характеристики:");
                        socketFacts = GetSocketFactsFromConsole();
                        CreateSocket(socketFacts, newSocketName).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.UpdateExistingSocket:
                        WritePaddedTop("Обновление существующего разъема, введите название разъема:");
                        var updatingSocketName = Console.ReadLine();
                        var updatingSocket = GetSocketByName(updatingSocketName).Result;
                        if (updatingSocket == null)
                        {
                            Console.WriteLine($"Разъем {updatingSocketName} не существует");
                            break;
                        }

                        Console.WriteLine($"Выбран разъем {updatingSocketName}");
                        WritePaddedTop("Обновление существующего разъема, введите характеристики:");
                        socketFacts = GetSocketFactsFromConsole();
                        UpdateSocket(socketFacts, updatingSocket).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.DeleteExistingSocket:
                        WritePaddedTop("Удаление существующего разъема, введите название разъема:");
                        var deletingSocketName = Console.ReadLine();
                        var deletingSocket = GetSocketByName(deletingSocketName).Result;
                        if (deletingSocket == null)
                        {
                            Console.WriteLine($"Разъем {deletingSocketName} не существует");
                            break;
                        }

                        DeleteSocketByName(deletingSocketName).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.GetSocketsInGroup:
                        WritePaddedTop("Получение разъемов группы, введите название группы:");
                        var groupName = Console.ReadLine();
                        GetSocketsByGroupName(groupName).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.AddSocketToGroup:
                        WritePaddedTop("Добавление существующего разъема в группу, введите название разъема:");
                        var requiredSocketName = Console.ReadLine();
                        var socket = GetSocketByName(requiredSocketName).Result;
                        if (socket == null)
                        {
                            Console.WriteLine($"Разъем {requiredSocketName} не существует");
                            break;
                        }

                        WritePaddedTop("Введите название группы:");
                        groupName = Console.ReadLine();
                        AddSocketToGroup(socket, groupName).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.RemoveSocketFromGroup:
                        WritePaddedTop("Удаление существующего разъема из группы, введите название разъема:");
                        requiredSocketName = Console.ReadLine();
                        socket = GetSocketByName(requiredSocketName).Result;
                        if (socket == null)
                        {
                            Console.WriteLine($"Разъем {requiredSocketName} не существует");
                            break;
                        }

                        WritePaddedTop("Введите название группы:");
                        groupName = Console.ReadLine();

                        RemoveSocketFromGroup(socket, groupName).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.GetSocketGroups:
                        WritePaddedTop("Получение всех групп");
                        GetSocketsGroups().ConfigureAwait(false);
                        break;

                    case ConsoleCommands.AddSocketGroup:
                        WritePaddedTop("Добавление новой группы, введите название группы:");
                        groupName = Console.ReadLine();
                        AddNewGroup(groupName).ConfigureAwait(false);
                        break;

                    case ConsoleCommands.RemoveSocketGroup:
                        WritePaddedTop("Удаление существующей группы, введите название группы:");
                        groupName = Console.ReadLine();
                        RemoveGroup(groupName).ConfigureAwait(false);
                        break;

                    default:
                        WritePaddedTop("Команда не распознана");
                        break;
                }

                PrintCommands();
            }
        }

        #region Decisions processing

        private async Task ForwardProcessing(FactSet factSet)
        {
            WritePaddedBottom($"Прямой продукционный вывод для {factSet}");
            var socketType = typeof(CustomSocket);
            var customSocket = new CustomSocket();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            try
            {
                var foundSockets = await Client.FindSocketsByParams(customSocket).ResponseStream.ToListAsync();
                WritePaddedTop("Возможные разъемы: " + string.Join(", ", foundSockets));
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
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

        private async Task LogicProcessing(FactSet factSet, string socketName)
        {
            WritePaddedBottom($"Логический вывод утверждения {socketName} при посылках {factSet}");
            var socketType = typeof(CustomSocket);
            var customSocket = new CustomSocket
            {
                SocketName = socketName
            };
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            try
            {
                var fuckingBullshit = await Client.IsParamsMatchSocketAsync(customSocket);
                var isCorrect = fuckingBullshit != null;
                WritePaddedTop(isCorrect ? $"Утверждение для {socketName} верно" : "Утверждение неверно");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task FuzzyProcessingMamdani(FactSet factSet)
        {
            WritePaddedBottom($"Нечеткий вывод (Мамдани) максимальной силы тока при разрыве цепи при известных {factSet}");

            var socketType = typeof(CustomSocket);
            var customSocket = new FuzzySocketParams();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            try
            {
                var amperageCircuit = await Client.FuzzyGetAmperageCircuitByParamsAsync(new FuzzySocketRequest
                {
                    Method = FuzzyMethod.Mamdani,
                    Socket = customSocket
                });
                WritePaddedTop($"Максимальная сила тока при разрыве цепи: {amperageCircuit.AmperageCircuit} мА");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task FuzzyProcessingSugeno(FactSet factSet)
        {
            WritePaddedBottom($"Нечеткий вывод (Сугэно) максимальной силы тока при разрыве цепи при известных {factSet}");
            var socketType = typeof(CustomSocket);
            var customSocket = new FuzzySocketParams();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            try
            {
                var amperageCircuit = await Client.FuzzyGetAmperageCircuitByParamsAsync(new FuzzySocketRequest
                {
                    Method = FuzzyMethod.Sugeno,
                    Socket = customSocket
                });
                WritePaddedTop($"Максимальная сила тока при разрыве цепи: {amperageCircuit.AmperageCircuit} мА");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task FuzzyNeuralProcessing(FactSet factSet)
        {
            WritePaddedBottom($"Нейро-нечеткий вывод (ANFIS) максимальной силы тока при разрыве цепи при известных {factSet}");
            var socketType = typeof(CustomSocket);
            var customSocket = new FuzzySocketParams();
            foreach (var fact in factSet)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(customSocket, fact.Value);

            try
            {
                var amperageCircuit = await Client.FuzzyGetAmperageCircuitByParamsAsync(new FuzzySocketRequest
                {
                    Method = FuzzyMethod.Neural,
                    Socket = customSocket
                });
                WritePaddedTop($"Максимальная сила тока при разрыве цепи: {amperageCircuit.AmperageCircuit} мА");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        #endregion

        #region Sockets/Groups manipulation methods

        private async Task<CustomSocket> GetSocketByName(string socketName)
        {
            var socketIdentity = new CustomSocketIdentity { SocketName = socketName };
            var socket = await Client.FindSocketByIdentityAsync(socketIdentity);
            return socket;
        }

        private async Task CreateSocket(List<Fact> socketFacts, string socketName)
        {
            var socketType = typeof(CustomSocket);
            var creatingSocket = new CustomSocket();
            foreach (var fact in socketFacts)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(creatingSocket, fact.Value);
            creatingSocket.SocketName = socketName;
            try
            {
                await Client.UpsertSocketAsync(creatingSocket);
                Console.WriteLine($"Разъем: {creatingSocket.SocketName} был успешно добавлен.");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task UpdateSocket(List<Fact> socketFacts, CustomSocket socket)
        {
            var socketType = typeof(CustomSocket);
            foreach (var fact in socketFacts)
                socketType.GetProperty(fact.Domain.ToString()).SetValue(socket, fact.Value);
            try
            {
                await Client.UpsertSocketAsync(socket);
                Console.WriteLine($"Разъем: {socket.SocketName} был успешно обновлен.");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task DeleteSocketByName(string socketName)
        {
            var socketIdentity = new CustomSocketIdentity { SocketName = socketName };
            try
            {
                await Client.DeleteSocketAsync(socketIdentity);
                Console.WriteLine($"Разъем: {socketName} был успешно удалён.");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task GetSocketsByGroupName(string groupName)
        {
            var groupIdentity = new SocketGroupIdentity { GroupName = groupName };
            try
            {
                var socketsInGroup = await Client.GetSocketsInGroup(groupIdentity).ResponseStream.ToListAsync();
                WritePaddedTop($"Разъемы принадлежащие группе {groupName}");
                var socketsNamesResultString = string.Empty;
                foreach (var socket in socketsInGroup)
                    socketsNamesResultString += $"{socket.SocketName}; ";
                Console.WriteLine(socketsNamesResultString);
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task AddSocketToGroup(CustomSocket socket, string groupName)
        {
            var socketJoinGroup = new CustomSocketIdentityJoinGroup
            {
                Socket = new CustomSocketIdentity { SocketName = socket.SocketName },
                Group = new SocketGroupIdentity { GroupName = groupName }
            };
            try
            {
                await Client.AddToSocketGroupAsync(socketJoinGroup);
                Console.WriteLine($"Разъем: {socket.SocketName} был успешно добавлен в группу {groupName}.");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task RemoveSocketFromGroup(CustomSocket socket, string groupName)
        {
            var socketJoinGroup = new CustomSocketIdentityJoinGroup()
            {
                Socket = new CustomSocketIdentity { SocketName = socket.SocketName },
                Group = new SocketGroupIdentity { GroupName = groupName }
            };
            try
            {
                await Client.RemoveFromSocketGroupAsync(socketJoinGroup);
                Console.WriteLine($"Разъем: {socket.SocketName} был успешно удалён из группы {groupName}.");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task GetSocketsGroups()
        {
            try
            {
                var groups = await Client.GetSocketGroups(new Empty()).ResponseStream.ToListAsync();
                var socketsGroupsResultString = string.Empty;
                foreach (var group in groups)
                    socketsGroupsResultString += $"{group.GroupName}; ";
                Console.WriteLine(socketsGroupsResultString);
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task AddNewGroup(string groupName)
        {
            var groupIdentity = new SocketGroupIdentity { GroupName = groupName };
            try
            {
                await Client.AddSocketGroupAsync(groupIdentity);
                Console.WriteLine($"Группа: {groupName} была успешно добавлена.");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        private async Task RemoveGroup(string groupName)
        {
            var groupIdentity = new SocketGroupIdentity { GroupName = groupName };
            try
            {
                await Client.DeleteSocketGroupAsync(groupIdentity);
                Console.WriteLine($"Группа: {groupName} была успешно удалена.");
            }
            catch (Exception e)
            {
                WritePaddedTop(e.Message);
            }
        }

        #endregion

        private static void PrintCommands()
        {
            WritePaddedTop("Выберите действие: ");
            Console.WriteLine($"{(int)ConsoleCommands.ForwardProcessing} - прямой продукционный вывод");
            Console.WriteLine($"{(int)ConsoleCommands.BackProcessing} - обратный продукционный вывод");
            Console.WriteLine($"{(int)ConsoleCommands.LogicProcessing} - логический вывод");
            Console.WriteLine($"{(int)ConsoleCommands.FuzzyProcessingMamdani} - нечеткий вывод (Мамдани)");
            Console.WriteLine($"{(int)ConsoleCommands.FuzzyProcessingSugeno} - нечеткий вывод (Сугэно)");
            Console.WriteLine($"{(int)ConsoleCommands.FuzzyNeuralProcessing} - нейро-нечеткий вывод (ANFIS)");
            Console.WriteLine($"{(int)ConsoleCommands.AddNewSocket} - добавление нового разъёма");
            Console.WriteLine($"{(int)ConsoleCommands.UpdateExistingSocket} - обновление существующего разъёма");
            Console.WriteLine($"{(int)ConsoleCommands.DeleteExistingSocket} - удаление существующего разъёма");
            WritePaddedBottom($"{(int)ConsoleCommands.Exit} - выход");
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

        private static List<Fact> GetSocketFactsFromConsole(List<SocketDomain> domains = null)
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
                case null:
                    throw new ArgumentNullException(nameof(input));
                case "":
                    throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default:
                    return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}