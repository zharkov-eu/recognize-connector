using System;
using System.Linq;
using System.Collections.Generic;

namespace ExpertSystem.Models.FuzzyLogic
{
    public class FuzzyFact : Fact
    {
        public new double Value;
        public new FuzzyDomain Domain;
        public Dictionary<int, double> ClusterDegree;
        
        public FuzzyFact(FuzzyDomain domain, double value, Dictionary<int, double> clusterDegree)
            : base(domain.Domain, value, typeof(double))
        {
            Value = value;
            Domain = domain;
            ClusterDegree = clusterDegree;
        }

        public int GetMostProbableCluster()
        {
            return ClusterDegree.OrderByDescending(p => p.Value).FirstOrDefault().Key;
        }

        public override string ToString()
        {
            var output = $"({{ {Domain.Domain}: {Value.ToString("0.#####")} }}: {{ ";
            output += string.Join(", ", ClusterDegree.Select(p => $"{p.Key}: {p.Value.ToString("0.#####")}"));
            return output + " })";
        }
    }
}