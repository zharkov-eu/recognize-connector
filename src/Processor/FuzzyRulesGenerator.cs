using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor
{
    public class FuzzyRulesGenerator
    {
        public List<FuzzyDomain> GetFuzzyDomains(List<CustomSocket> sockets)
        {
            var fuzzyDomains = new List<FuzzyDomain>();
            var customSocketType = typeof(CustomSocket);

            foreach (var domain in GetFuzzySocketDomains())
            {
                Type type = SocketDomainType[domain];
                var domainValues = sockets.Select(p => customSocketType.GetField(domain.ToString()).GetValue(p))
                    .Where(p => !SocketDefaultValue[type].Equals(p));

                fuzzyDomains.Add(new FuzzyDomain(
                    domain, new FuzzyDomainOption { Min = domainValues.Min(), Max = domainValues.Max() }
                ));
            }
            return fuzzyDomains;
        }
    }
}