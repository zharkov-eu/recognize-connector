using System;
using System.Collections.Generic;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models
{
    public struct FuzzyDomainOption {
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
        }
    }
}