using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class LogicFact : Fact
    {
        bool Negation { get; set; }
        LogicOperation RightOperation { get; set; }

        public LogicFact(string domain, string value) : base(domain, value) {
            Negation = false;
            RightOperation = LogicOperation.None;
        }

        public LogicFact(string domain, string value, LogicOperation operation, bool negation = false) : base(domain, value) {
            Negation = negation;
            RightOperation = operation;
        }
    }
}