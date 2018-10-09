using System;
using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class FuzzyStatement
    {
        public HashSet<FuzzyRule> Rules;
        public Func<CustomSocket, double> Result;

        public FuzzyStatement(HashSet<FuzzyRule> rules, Func<CustomSocket, double> result)
        {
            Rules = rules;
            Result = result;
        }
    }

    public class FuzzyRule {
        public FuzzyDomain Domain;
        public int Cluster;

        public FuzzyRule(FuzzyDomain domain, int cluster)
        {
            Domain = domain;
            Cluster = cluster;
        }
    }
}