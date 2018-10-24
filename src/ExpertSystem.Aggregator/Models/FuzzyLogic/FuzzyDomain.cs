using System;
using System.Collections.Generic;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Models.FuzzyLogic
{
    public struct FuzzyDomainOption
    {
        public int ClusterCount;
        public object Min;
        public object Max;
    }

    public class FuzzyDomain
    {
        public readonly HashSet<int> Clusters;
        public readonly CustomSocketDomain.SocketDomain Domain;
        public readonly Type Type;
        public object Max;
        public object Min;

        public FuzzyDomain(CustomSocketDomain.SocketDomain domain, FuzzyDomainOption options)
        {
            Domain = domain;
            Type = CustomSocketDomain.SocketDomainType[Domain];
            Min = options.Min;
            Max = options.Max;
            Clusters = new HashSet<int>();

            for (var i = 0; i < options.ClusterCount; i++)
                Clusters.Add(i);
        }
    }
}