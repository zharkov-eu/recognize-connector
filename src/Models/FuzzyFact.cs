using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class FuzzyFact : Fact
    {
        public new FuzzyDomain Domain;
        public Dictionary<int, float> ClusterDegree;
        
        public FuzzyFact(FuzzyDomain domain, object value) 
            : base(domain.Domain, value)
        {
            Domain = domain;
            ClusterDegree = new Dictionary<int, float>();
        }
    }
}