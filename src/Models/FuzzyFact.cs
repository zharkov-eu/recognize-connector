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

    public class FuzzyFactSet
    {
        public HashSet<FuzzyFact> Facts;
    }
}