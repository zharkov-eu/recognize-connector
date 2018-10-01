using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.LogicOperation;

namespace ExpertSystem.Processor.LogicProcessor
{
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
            HashSet<LinkedList<LogicFact>> statements = new HashSet<LinkedList<LogicFact>>();

            // Добавляем входные параметры, объединенные союзом "или"
            LinkedList<LogicFact> logicInputFacts;
            foreach (var inputFact in inputFacts.Facts.Where(p => !p.IsDefaultValue()))
            {
                logicInputFacts = new LinkedList<LogicFact>();
                logicInputFacts.AddLast(new LogicFact(inputFact.Domain, inputFact.Value, inputFact.Type, Operation.None));
                statements.Add(logicInputFacts);
            }
            
            // Добавляем имеющиеся правила
            foreach (var factStatement in _facts)
                statements.Add(factStatement);

            // Добавляем отрицание утверждения
            LinkedList<LogicFact> socketNegation = new LinkedList<LogicFact>();
            socketNegation.AddLast(new LogicFact("SocketName", socketName, typeof(string), Operation.None, true));
            statements.Add(socketNegation);

            // Получаем конъюнктивно нормальную форму
            HashSet<LinkedList<LogicFact>> cnfStatements = new HashSet<LinkedList<LogicFact>>();
            foreach (var statement in statements)
                cnfStatements.Add(LogicFact.ConjuctionNormalFrom(statement));

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