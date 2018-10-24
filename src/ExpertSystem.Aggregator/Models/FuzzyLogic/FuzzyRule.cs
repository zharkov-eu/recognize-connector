using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExpertSystem.Aggregator.Models.FuzzyLogic
{
    public class FuzzyRule
    {
        public readonly FuzzyDomain Domain;
        public readonly int Cluster;
        public double Degree;
        public double Value;

        public FuzzyRule(FuzzyDomain domain, int cluster)
        {
            Domain = domain;
            Cluster = cluster;
        }

        public FuzzyRule SetFact(FuzzyFact fact)
        {
            if (fact == null)
                throw new Exception($"FuzzyRule: fact is empty for domain {Domain}");
            Degree = fact.ClusterDegree[Cluster];
            Value = fact.Value;
            return this;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FuzzyRule rule))
                return false;
            return Domain.Equals(rule.Domain)
                   && Cluster.Equals(rule.Cluster)
                   && Degree.Equals(rule.Degree)
                   && Value.Equals(rule.Value);
        }

        public override int GetHashCode()
        {
            var hash = 19;
            hash += hash * 37 + Domain.GetHashCode();
            hash += hash * 37 + Cluster.GetHashCode();
            hash += hash * 37 + Degree.GetHashCode();
            hash += hash * 37 + Value.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            if (Degree == default(double) && Value == default(double))
                return $"{Domain.Domain}({Cluster})";
            return $"{Domain.Domain}({Cluster}: {Degree:0.#####}, {Value:0.#####})";
        }
    }

    public class FuzzyRuleSet : IEnumerable<FuzzyRule>
    {
        private readonly HashSet<FuzzyRule> _rules;

        public FuzzyRuleSet(params FuzzyRule[] rules)
        {
            _rules = new HashSet<FuzzyRule>();
            Add(rules);
        }

        public IEnumerator<FuzzyRule> GetEnumerator()
        {
            return _rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(params FuzzyRule[] rules)
        {
            foreach (var rule in rules)
                _rules.Add(rule);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FuzzyRuleSet rules))
                return false;
            return _rules.SetEquals(rules._rules);
        }

        public override int GetHashCode()
        {
            var hash = 19;
            return hash * 37 + _rules.GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(" ^ ", _rules.Select(it => it.ToString()));
        }
    }
}