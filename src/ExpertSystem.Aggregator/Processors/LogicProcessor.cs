using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Aggregator.Models.CommonLogic;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Processors
{
    public class LogicFactSet : HashSet<LinkedList<LogicFact>>
    {
        public override string ToString()
        {
            var output = "";
            foreach (var statement in this)
            {
                output += string.Join(" ", statement);
                if (this.Last() != statement)
                    output += ", ";
            }

            return output;
        }
    }

    public class LogicProcessor : AbstractProcessor
    {
        private readonly List<LinkedList<LogicFact>> _facts;

        public LogicProcessor(List<LinkedList<LogicFact>> facts, ProcessorOptions options)
            : base(options)
        {
            _facts = facts;
        }

        public bool Processing(FactSet inputFacts, string socketName)
        {
            var statements = new LogicFactSet();

            // Добавляем входные параметры, объединенные союзом "и"
            var inputDomains = new List<CustomSocketDomain.SocketDomain>();
            foreach (var inputFact in inputFacts.Facts.Where(p => !p.IsDefaultValue()))
            {
                inputDomains.Add(inputFact.Domain);
                var logicInputFacts = new LinkedList<LogicFact>();
                logicInputFacts.AddLast(
                    new LogicFact(inputFact.Domain, inputFact.Value, LogicOperation.Operation.None));
                statements.Add(logicInputFacts);
            }

            // Добавляем имеющиеся правила (отфильтрованные по входным св-м домена)
            foreach (var factStatement in _facts)
            {
                var statementFiltered =
                    new LinkedList<LogicFact>(factStatement.Where(p => inputDomains.Contains(p.Domain)));
                if (statementFiltered.Count == 0)
                    continue;

                statementFiltered.Last.Value.RightOperation = LogicOperation.Operation.Implication;
                statementFiltered.AddLast(factStatement.Last.Value);
                statements.Add(statementFiltered);
            }

            // Добавляем отрицание утверждения
            var socketNegation = new LinkedList<LogicFact>();
            socketNegation.AddLast(new LogicFact(CustomSocketDomain.SocketDomain.SocketName, socketName,
                LogicOperation.Operation.None,
                true));
            statements.Add(socketNegation);

            // Выводим отладочную информацию первого шага
            DebugWrite("".PadLeft(40, '-'));
            DebugWrite("Начальная диспозиция: " + statements);

            // Получаем конъюнктивно-нормальную форму
            var cnfStatements = new LogicFactSet();
            foreach (var statement in statements)
                cnfStatements.Add(LogicFact.ConjunctionNormalFrom(statement));

            // Выводим отладочную информацию КНФ
            DebugWrite("".PadLeft(40, '-'));
            DebugWrite("Конъюнктивная нормальная форма: " + cnfStatements);
            return Resolve(
                cnfStatements, new LogicFact(CustomSocketDomain.SocketDomain.SocketName, socketName, LogicOperation.Operation.None)
            );
        }

        public bool Resolve(LogicFactSet statements, LogicFact result)
        {
            var currentStatements = new LogicFactSet();

            // Если множество утверждений пусто - значит утверждение неверно
            if (statements.Count == 0)
                return false;
            var currentStatement = statements.First();

            // Проверим, не найдено ли утверждение посылки
            if (currentStatement.Count == 1 && currentStatement.First.Value.Equals(result))
                return true;

            foreach (var statement in statements)
            {
                // Пропустим текущий элемент
                if (statement == currentStatement)
                    continue;

                // Проверим, не найдено ли утверждение посылки
                if (statement.Count == 1 && statement.First.Value.Equals(result))
                    return true;

                foreach (var currentFact in currentStatement)
                {
                    var domainExist = false;
                    foreach (var fact in statement)
                        if (fact.Domain == currentFact.Domain && fact.Negation != currentFact.Negation)
                        {
                            domainExist = true;
                            if (fact.Value.Equals(currentFact.Value))
                                currentStatements.Add(
                                    new LinkedList<LogicFact>(statement.Where(p => !Equals(p, fact)))
                                );
                            break;
                        }

                    if (!domainExist)
                        currentStatements.Add(statement);
                }
            }

            // Отладочный вывод
            DebugWrite("".PadLeft(40, '-'));
            DebugWrite(currentStatements.ToString());

            return Resolve(currentStatements, result);
        }
    }
}