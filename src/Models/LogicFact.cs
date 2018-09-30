namespace ExpertSystem.Models
{
    public class LogicFact : Fact
    {
        public bool Negation { get; set; }
        public LogicOperation RightOperation { get; set; }

        public LogicFact(string domain, string value) : base(domain, value)
        {
            Negation = false;
            RightOperation = LogicOperation.None;
        }

        public LogicFact(string domain, string value, LogicOperation operation, bool negation = false) 
            : base(domain, value)
        {
            Negation = negation;
            RightOperation = operation;
        }
    }
}