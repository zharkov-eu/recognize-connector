using System;
using System.Collections.Generic;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models.FuzzyLogic
{
    public struct FuzzyDomainOption
    {
        public int ClusterCount;
        public object Min;
        public object Max;
    }

    public class FuzzyDomain
    {
        public SocketDomain Domain;
        public Type Type;
        public object Min;
        public object Max;
        public HashSet<int> Clusters;

        public FuzzyDomain(SocketDomain domain, FuzzyDomainOption options)
        {
            Domain = domain;
            Type = SocketDomainType[Domain];
            Min = options.Min;
            Max = options.Max;
            Clusters = new HashSet<int>();

            for (var i = 0; i < options.ClusterCount; i++)
                Clusters.Add(i);
        }
    }
}