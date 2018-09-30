using System;
using System.Linq;
using System.Collections.Generic;
using static ExpertSystem.Models.LogicOperation;

namespace ExpertSystem.Models
{
    public class LogicFact : Fact
    {
        /// <summary>
        /// Возвращает конъюктивно нормальную форму
        /// <summary>
        public static LinkedList<LogicFact> ConjunctionNormalForm(LinkedList<LogicFact> facts, int priority = 100) {
            if (facts.Count <= 1) return facts;
            int maxPriority = facts.Select(fact => Priority[fact.RightOperation]).Max();
            foreach (var fact in facts)
            {
                if (Priority[fact.RightOperation] == Math.Min(priority, maxPriority))
                {
                    switch (fact.RightOperation)
                    {
                        case Operations.Implication:
                            break;
                        case Operations.Conjunction:
                            break;
                        case Operations.Disjunction:
                            break;
                        case Operations.None:
                            return facts;
                    }
                }
            }

            return ConjunctionNormalForm(facts, maxPriority - 1);
        }

        public bool Negation { get; set; }
        public Operations RightOperation { get; set; }

        public LogicFact(string domain, string value) : base(domain, value)
        {
            Negation = false;
            RightOperation = LogicOperation.Operations.None;
        }

        public LogicFact(string domain, string value, Operations operation, bool negation = false) 
            : base(domain, value)
        {
            Negation = negation;
            RightOperation = operation;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var fact = obj as LogicFact;
            if (fact == null)
                return false;
            return Domain == fact.Domain &&
                   Value == fact.Value &&
                   Negation == fact.Negation &&
                   RightOperation == fact.RightOperation;
        }
        
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            hashCode += 27 * Negation.GetHashCode();
            hashCode += 27 * RightOperation.GetHashCode();
            return hashCode;
        }
    }
}