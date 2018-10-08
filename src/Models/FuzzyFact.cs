using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public struct ClusterDegree
	{
		public int Cluster;
		public double Degree;
	}

    public class FuzzyFact : Fact
    {
        public new FuzzyDomain Domain;
        public List<ClusterDegree> ClusterDegree;
        
        public FuzzyFact(FuzzyDomain domain, object value) 
            : base(domain.Domain, value)
        {
            Domain = domain;
            ClusterDegree = new List<ClusterDegree>();
        }
    }

    public class FuzzyFactSet
    {
        public HashSet<FuzzyFact> Facts;
    }
}