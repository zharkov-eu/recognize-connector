using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Models.CommonLogic
{
    public struct LogicFactLevel
    {
        public int Depth;
        public bool Negation;

        public override bool Equals(object obj)
        {
            if (!(obj is LogicFactLevel))
                return false;

            var fact = (LogicFactLevel) obj;
            return Depth == fact.Depth && Negation == fact.Negation;
        }

        public override int GetHashCode()
        {
            var hashCode = 17;
            hashCode += Depth.GetHashCode();
            hashCode += Negation.GetHashCode();
            return hashCode;
        }
    }

    public class LogicFact : Fact
    {
        public LogicFact(CustomSocketDomain.SocketDomain domain, object value) : base(domain, value)
        {
            Negation = false;
            RightOperation = LogicOperation.Operation.None;
            Level = new LogicFactLevel {Depth = 0, Negation = false};
        }

        public LogicFact(CustomSocketDomain.SocketDomain domain, object value, LogicOperation.Operation operation,
            bool negation = false)
            : base(domain, value)
        {
            Negation = negation;
            RightOperation = operation;
            Level = new LogicFactLevel {Depth = 0, Negation = false};
        }

        public bool Negation { get; set; }
        public LogicOperation.Operation RightOperation { get; set; }
        public LogicFactLevel Level { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is LogicFact fact))
                return false;
            return base.Equals(obj) &&
                   Negation == fact.Negation &&
                   RightOperation == fact.RightOperation;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode += 27 * Negation.GetHashCode();
            hashCode += 27 * RightOperation.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            var output = Negation ? "~" : "";
            output += $"({Domain}: {Value})";
            return string.IsNullOrEmpty(LogicOperation.GetOutput(RightOperation))
                ? output
                : output + $" {LogicOperation.GetOutput(RightOperation)}";
        }
    }
}