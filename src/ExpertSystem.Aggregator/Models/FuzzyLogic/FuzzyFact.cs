using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Models.FuzzyLogic
{
    public class FuzzyFact : Fact
    {
        public Dictionary<int, double> ClusterDegree;
        public new readonly FuzzyDomain Domain;
        public new readonly double Value;

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
            var output = $"({{ {Domain.Domain}: {Value:0.#####} }}: {{ ";
            output += string.Join(", ", ClusterDegree.Select(p => $"{p.Key}: {p.Value:0.#####}"));
            return output + " })";
        }
    }
}