using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;

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
            LinkedList<LogicFact> logicInputFacts = new LinkedList<LogicFact>();
            foreach (var inputFact in inputFacts.Facts)
            {
                logicInputFacts.AddLast(new LogicFact(
                    inputFact.Domain,
                    inputFact.Value,
                    inputFacts.Facts.Last() == inputFact ? LogicOperation.Operations.None : LogicOperation.Operations.Conjunction)
                );
            }

            LinkedList<LogicFact> socketNegation = new LinkedList<LogicFact>();
            socketNegation.AddLast(new LogicFact("SocketName", socketName, LogicOperation.Operations.None, true));

            HashSet<LinkedList<LogicFact>> statements = new HashSet<LinkedList<LogicFact>>();
            statements.Add(logicInputFacts);
            foreach (var factStatement in _facts)
                statements.Add(factStatement);
            statements.Add(socketNegation);

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