using System.Collections.Generic;
using System.Linq;

namespace ExpertSystem.Models
{
    public class Fact : IEqualityComparer<Fact>
    {
        public string Domain { get; set; }
        public string Value { get; set; }

        public Fact(string domain, string value)
        {
            Domain = domain;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) 
                return false;
            var fact = obj as Fact;
            if (fact == null) 
                return false;
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

        public override string ToString()
        {
            return $"{Domain}: {Value}";
        }
    }

    public class FactSet
    {
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
            if (obj == null) 
                return false;
            var facts = obj as FactSet;
            if (facts == null) 
                return false;
            return Facts.SetEquals(facts.Facts);
        }

        public override int GetHashCode()
        {
            var hash = 19;
            return hash * 37 + Facts.GetHashCode();
        }

        public override string ToString()
        {
            return "{ " + string.Join(", ", Facts.Select(it => it.ToString())) + " }";
        }
    }
}