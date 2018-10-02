using System;
using System.Linq;
using System.Collections.Generic;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models
{
    public class Fact : IEqualityComparer<Fact>
    {
        public SocketDomain Domain { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }

        public Fact(SocketDomain domain, object value)
        {
            Domain = domain;
            Type = SocketDomainType[Domain];
            if (value.GetType() != Type)
                throw new Exception(
                    $"Expected type: {Type} for {Domain}: get {value.GetType()} with value {value.ToString()}"
                );
            Value = value;
        }

        public bool IsDefaultValue()
        {
            return Value.Equals(SocketDefaultValue[Type]);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var fact = obj as Fact;
            if (fact == null)
                return false;
            return Type.Equals(fact.Type) && Domain.Equals(fact.Domain) && Value.Equals(fact.Value);
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
            hash = hash * 37 + Type.GetHashCode();
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