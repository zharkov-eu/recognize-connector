using System.Collections.Generic;

namespace ExpertSystem.Models
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
    }

    public class FuzzyFactClustered : FuzzyFact
    {
        public KeyValuePair<int, double> Cluster;
        
        public FuzzyFactClustered(FuzzyDomain domain, object value, Dictionary<int, double> clusterDegree, int cluster)
            : base(domain, value, clusterDegree)
        {
            Cluster = new KeyValuePair<int, double>(cluster, ClusterDegree[cluster]);
        }
    }
}