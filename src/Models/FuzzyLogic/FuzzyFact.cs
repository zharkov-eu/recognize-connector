using System.Linq;
using System.Collections.Generic;

namespace ExpertSystem.Models.FuzzyLogic
{
    public class FuzzyFact : Fact
    {
        public new FuzzyDomain Domain;
        public Dictionary<int, double> ClusterDegree;
        
        public FuzzyFact(FuzzyDomain domain, object value, Dictionary<int, double> clusterDegree)
            : base(domain.Domain, value)
        {
            Domain = domain;
            ClusterDegree = clusterDegree;
        }

        public int GetMostProbableCluster()
        {
            return ClusterDegree.OrderByDescending(p => p.Value).FirstOrDefault().Key;
        }
    }
}