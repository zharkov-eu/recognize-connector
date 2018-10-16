using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExpertSystem.Models.FuzzyLogic
{
    public class FuzzyRule
    {
        public int Cluster;
        public double Degree;
        public FuzzyDomain Domain;
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
            if (obj == null)
                return false;
            var rule = obj as FuzzyRule;
            if (rule == null)
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
            return $"{Domain.Domain}({Cluster}: {Degree.ToString("0.#####")}, {Value.ToString("0.#####")})";
        }
    }

    public class FuzzyRuleSet : IEnumerable<FuzzyRule>
    {
        public HashSet<FuzzyRule> Rules;

        public FuzzyRuleSet(params FuzzyRule[] rules)
        {
            Rules = new HashSet<FuzzyRule>();
            Add(rules);
        }

        public IEnumerator<FuzzyRule> GetEnumerator()
        {
            return Rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(params FuzzyRule[] rules)
        {
            foreach (var rule in rules)
                Rules.Add(rule);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var rules = obj as FuzzyRuleSet;
            if (rules == null)
                return false;
            return Rules.SetEquals(rules.Rules);
        }

        public override int GetHashCode()
        {
            var hash = 19;
            return hash * 37 + Rules.GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(" ^ ", Rules.Select(it => it.ToString()));
        }
    }
}