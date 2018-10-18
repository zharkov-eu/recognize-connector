using System;
using System.Collections.Generic;
using System.Linq;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models.FuzzyLogic
{
    public abstract class FuzzyStatement
    {
        public FuzzyRuleSet Rules;

        public FuzzyStatement(FuzzyRuleSet rules)
        {
            Rules = rules;
        }

        public FuzzyStatement SetRulesFacts(List<FuzzyFact> facts)
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
        public Func<CustomSocket, double> Result;

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
        public FuzzyRule Result;

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