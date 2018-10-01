using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.LogicOperation;

namespace ExpertSystem.Processor.LogicProcessor
{
    public class LogicFactSet : HashSet<LinkedList<LogicFact>>
    {
        public override string ToString()
        {
            string output = "";
            foreach(var statement in this)
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
        private List<LinkedList<LogicFact>> _facts;

        public LogicProcessor(List<LinkedList<LogicFact>> facts, ProcessorOptions options) 
            : base(options)
        {
            _facts = facts;
        }

        public bool Processing(FactSet inputFacts, string socketName)
        {
            LogicFactSet statements = new LogicFactSet();

            // Добавляем входные параметры, объединенные союзом "и"
            List<string> inputDomains = new List<string>();
            LinkedList<LogicFact> logicInputFacts;
            foreach (var inputFact in inputFacts.Facts.Where(p => !p.IsDefaultValue()))
            {
                inputDomains.Add(inputFact.Domain);
                logicInputFacts = new LinkedList<LogicFact>();
                logicInputFacts.AddLast(new LogicFact(inputFact.Domain, inputFact.Value, inputFact.Type, Operation.None));
                statements.Add(logicInputFacts);
            }
            
            // Добавляем имеющиеся правила (отфильтрованные по входным св-м домена)
            foreach (var factStatement in _facts)
            {
                var statementFiltered = new LinkedList<LogicFact>(factStatement.Where(p => inputDomains.Contains(p.Domain)));
                if (statementFiltered.Count == 0) continue;

                statementFiltered.Last.Value.RightOperation = Operation.Implication;
                statementFiltered.AddLast(factStatement.Last.Value);
                statements.Add(statementFiltered);
            }

            // Добавляем отрицание утверждения
            LinkedList<LogicFact> socketNegation = new LinkedList<LogicFact>();
            socketNegation.AddLast(new LogicFact("SocketName", socketName, typeof(string), Operation.None, true));
            statements.Add(socketNegation);

            // Выводим отладочную информацию первого шага
            debug("".PadLeft(40, '-'));
            debug("Начальная диспозиция: " + statements.ToString());

            // Получаем конъюнктивно-нормальную форму
            LogicFactSet cnfStatements = new LogicFactSet();
            foreach (var statement in statements)
                cnfStatements.Add(LogicFact.ConjuctionNormalFrom(statement));

            // Выводим отладочную информацию КНФ
            debug("".PadLeft(40, '-'));
            debug("Конъюнктивная нормальная форма: " + cnfStatements.ToString());

            return Resolve(cnfStatements, new LogicFact("SocketName", socketName, typeof(string), Operation.None));
        }

        public bool Resolve(LogicFactSet statements, LogicFact result)
        {
            LogicFactSet currentStatements = new LogicFactSet();

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
                if (statement == currentStatement) continue;

                // Проверим, не найдено ли утверждение посылки
                if (statement.Count == 1 && statement.First.Value.Equals(result)) {
                    return true;
                }

                foreach (var currentFact in currentStatement)
                {
                    bool domainExist = false;
                    foreach (var fact in statement)
                    {
                        if (fact.Domain == currentFact.Domain && fact.Negation != currentFact.Negation)
                        {
                            domainExist = true;
                            if (fact.Value.Equals(currentFact.Value))
                                currentStatements.Add(new LinkedList<LogicFact>(statement.Where(p => p != fact)));
                            break;
                        }
                    }
                    if (!domainExist) currentStatements.Add(statement);
                }
            }

            // Отладочный вывод
            debug("".PadLeft(40, '-'));
            debug(currentStatements.ToString());

            return Resolve(currentStatements, result);
        }
    }
}