using System;
using System.Collections.Generic;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Client.Models.FuzzyLogic
{
    public struct FuzzyDomainOption
    {
        public int ClusterCount;
        public object Min;
        public object Max;
    }

    public class FuzzyDomain
    {
        public HashSet<int> Clusters;
        public SocketDomain Domain;
        public object Max;
        public object Min;
        public Type Type;

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