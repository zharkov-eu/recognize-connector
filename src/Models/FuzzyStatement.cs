using System;
using System.Linq;
using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public abstract class FuzzyStatement {
        public HashSet<FuzzyRule> Rules;

        public FuzzyStatement(HashSet<FuzzyRule> rules)
        {
            Rules = rules;
        }

        public FuzzyStatement SetRulesFacts(List<FuzzyFact> facts)
        {
            Dictionary<FuzzyDomain, FuzzyFact> domainFacts = facts.ToDictionary(p => p.Domain);
            foreach (var rule in Rules)
                rule.SetFact(domainFacts[rule.Domain]);
            return this;
        }

        public double GetRulesDegree()
        {
            return Rules.Min(p => p.Degree);
        }
    }

    public class FuzzyFuncStatement : FuzzyStatement
    {
        public Func<CustomSocket, double> Result;

        public FuzzyFuncStatement(HashSet<FuzzyRule> rules, Func<CustomSocket, double> result) : base(rules)
        {
            Result = result;
        }
    }

    public struct FuzzyFuncProcessed {
        public double Degree;
        public double Result;
    }

    public class FuzzyRuleStatement : FuzzyStatement
    {
        public FuzzyRule Result;

        public FuzzyRuleStatement(HashSet<FuzzyRule> rules, FuzzyRule result) : base(rules)
        {
            Result = result;
        }

        public FuzzyRuleStatement SetResultFact(FuzzyFact fact)
        {
            Result.Degree = Math.Min(GetRulesDegree(), Result.SetFact(fact).Degree);
            return this;
        }
    }

    public class FuzzyRule {
        public FuzzyDomain Domain;
        public int Cluster;
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
            Value = Convert.ToDouble(fact.Value);
            return this;
        }
    }
}