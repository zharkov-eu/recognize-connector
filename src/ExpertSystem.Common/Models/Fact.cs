using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExpertSystem.Common.Models
{
    public class Fact : IEqualityComparer<Fact>
    {
        public Fact(CustomSocketDomain.SocketDomain domain, object value) : this(domain, value,
            CustomSocketDomain.SocketDomainType[domain])
        {}

        public Fact(CustomSocketDomain.SocketDomain domain, object value, Type type)
        {
            Domain = domain;
            Type = type;
            if (value.GetType() != Type)
                throw new Exception(
                    $"Expected type: {Type} for {Domain}: get {value.GetType()} with value {value}"
                );
            Value = value;
        }

        public CustomSocketDomain.SocketDomain Domain { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }

        public bool Equals(Fact a, Fact b)
        {
            return a.Equals(b);
        }

        public int GetHashCode(Fact fact)
        {
            return fact.GetHashCode();
        }

        public bool IsDefaultValue()
        {
            return Value.Equals(CustomSocketDomain.SocketDefaultValue[Type]);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Fact fact))
                return false;
            return Type.Equals(fact.Type) && Domain.Equals(fact.Domain) && Value.Equals(fact.Value);
        }

        public override int GetHashCode()
        {
            var hash = 19;
            hash = hash * 37 + Domain.GetHashCode();
            hash = hash * 37 + Value.GetHashCode();
            hash = hash * 37 + Type.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"{Domain}: {Value}";
        }
    }

    public class FactSet : IEnumerable<Fact>
    {
        public readonly HashSet<Fact> Facts;

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

        public IEnumerator<Fact> GetEnumerator()
        {
            return Facts.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return obj is FactSet facts && Facts.SetEquals(facts.Facts);
        }

        public override int GetHashCode()
        {
            int hash = 19;
            return hash * 37 + Facts.GetHashCode();
        }

        public override string ToString()
        {
            return "{ " + string.Join(", ", Facts.Select(it => it.ToString())) + " }";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}