using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Aggregator.Models.FuzzyLogic
{
    public abstract class FuzzyStatement
    {
        public readonly FuzzyRuleSet Rules;

        protected FuzzyStatement(FuzzyRuleSet rules)
        {
            Rules = rules;
        }

        public FuzzyStatement SetRulesFacts(IEnumerable<FuzzyFact> facts)
        {
            var domainFacts = facts.ToDictionary(p => p.Domain);
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
        public readonly Func<CustomSocket, double> Result;

        public FuzzyFuncStatement(FuzzyRuleSet rules, Func<CustomSocket, double> result) : base(rules)
        {
            Result = result;
        }

        public override string ToString()
        {
            return $"{Rules} -> f(NoC,SL,SW)";
        }
    }

    public struct FuzzyFuncProcessed
    {
        public double Degree;
        public double Result;
    }

    public class FuzzyRuleStatement : FuzzyStatement
    {
        public readonly FuzzyRule Result;

        public FuzzyRuleStatement(FuzzyRuleSet rules, FuzzyRule result) : base(rules)
        {
            Result = result;
        }

        public FuzzyRuleStatement SetResultFact(FuzzyFact fact)
        {
            Result.Degree = Math.Min(GetRulesDegree(), Result.SetFact(fact).Degree);
            return this;
        }

        public override string ToString()
        {
            return $"{Rules} -> {Result}";
        }
    }
}