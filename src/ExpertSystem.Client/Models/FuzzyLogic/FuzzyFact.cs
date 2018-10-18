using System.Collections.Generic;
using System.Linq;

namespace ExpertSystem.Client.Models.FuzzyLogic
{
    public class FuzzyFact : Fact
    {
        public Dictionary<int, double> ClusterDegree;
        public new FuzzyDomain Domain;
        public new double Value;

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