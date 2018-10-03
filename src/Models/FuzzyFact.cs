using System.Collections.Generic;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models
{
    public class FuzzyFact : Fact
    {
        new public FuzzyDomain Domain;
        public Dictionary<int, float> ClusterDegree;
        
        public FuzzyFact(FuzzyDomain domain, object value) : base(domain.Domain, value)
        {
            Domain = domain;
            ClusterDegree = new Dictionary<int, float>();
        }
    }
}