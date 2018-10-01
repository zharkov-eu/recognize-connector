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
            debug("Начальная диспозиция: " + statements.ToString());

            // Получаем конъюнктивно-нормальную форму
            HashSet<LinkedList<LogicFact>> cnfStatements = new HashSet<LinkedList<LogicFact>>();
            foreach (var statement in statements)
                cnfStatements.Add(LogicFact.ConjuctionNormalFrom(statement));

            // Выводим отладочную информацию КНФ
            debug("Конъюнктивно-нормальная форма: " + statements.ToString());

            return false;
        }

        public HashSet<LinkedList<LogicFact>> Resolve(HashSet<LinkedList<LogicFact>> statements)
        {
            var currentStatement = statements.First();
            foreach (var statement in statements)
            {
                if (statements.First() == statement) continue;
                foreach (var currentFact in currentStatement)
                    foreach (var fact in statement)
                        if (fact.Domain == currentFact.Domain && fact.Value == currentFact.Value && fact.Negation != currentFact.Negation)
                        {
                            
                        }
            }

            return statements;
        }
    }
}