namespace ExpertSystem.Models
{
    public class Fact {
        public string Domain { get; set; }
        public string Value { get; set; }

        public Fact(string domain, string value) {
            Domain = domain;
            Value = value;
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            Fact fact = obj as Fact;
            if (fact as Fact == null) return false;
            return Domain == fact.Domain && Value == fact.Value;
        }

        public override int GetHashCode() {
            var hash = 19;
            hash = hash * 37 + Domain.GetHashCode();
            hash = hash * 37 + Value.GetHashCode();
            return hash;
        }
    }
}