using System.Linq;
using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class Fact : IEqualityComparer<Fact> {
        public string Domain { get; set; }
        public string Value { get; set; }

        public Fact(string domain, string value)
        {
            Domain = domain;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Fact fact = obj as Fact;
            if (fact as Fact == null) return false;
            return Domain == fact.Domain && Value == fact.Value;
        }

        public bool Equals(Fact a, Fact b)
        {
            return a.Equals(b);
        }

        public override int GetHashCode()
        {
            var hash = 19;
            hash = hash * 37 + Domain.GetHashCode();
            hash = hash * 37 + Value.GetHashCode();
            return hash;
        }

        public int GetHashCode(Fact fact)
        {
            return fact.GetHashCode();
        }
    }

    public class FactSet {
        public HashSet<Fact> Facts;

        public FactSet(params Fact[] facts)
        {
            Facts = new HashSet<Fact>();
            foreach (var fact in facts)
                Facts.Add(fact);
        }

        public void Add(params Fact[] facts)
        {
            foreach (var fact in facts)
                Facts.Add(fact);
        }

        public Fact[] ToArray()
        {
            return Facts.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            FactSet facts = obj as FactSet;
            if (facts as FactSet == null) return false;
            return Facts.SetEquals(facts.Facts);
        }

        public override int GetHashCode()
        {
            var hash = 19;
            return hash * 37 + Facts.GetHashCode();
        }
    }
}