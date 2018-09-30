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
        public static LinkedList<LogicFact> ConjuctionNormalFrom(LinkedList<LogicFact> facts) {
            if (facts.Count <= 1) return facts;
            return facts;
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
    }
}