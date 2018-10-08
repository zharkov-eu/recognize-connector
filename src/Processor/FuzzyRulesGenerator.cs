using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Services;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor
{
    public class FuzzyRulesGenerator
    {
        public List<FuzzyStatement> GetFuzzyStatements(List<CustomSocket> sockets)
        {
            List<FuzzyStatement> statements = new List<FuzzyStatement>();
            foreach (var domain in _domainFacts.Keys)
            {
                if (!domains.Contains(domain.Domain))
                    continue;
                
                foreach (var fact in _domainFacts[domain])
                {

                }
            }
            return statements;
        }

        public Dictionary<FuzzyDomain, List<FuzzyFact>> GetFuzzyFacts(List<FuzzyDomain> domains, List<CustomSocket> sockets)
        {
            var fuzzyFacts = new Dictionary<FuzzyDomain, List<FuzzyFact>>();
            var customSocketType = typeof(CustomSocket);

            foreach (var domain in domains)
            {
                Type type = SocketDomainType[domain.Domain];
                List<double> domainValues = sockets
                    .Select(p => (double) customSocketType.GetField(domain.ToString()).GetValue(p))
                    .Where(p => !SocketDefaultValue[type].Equals(p))
                    .ToList();
                
                List<FuzzyFact> factList = new List<FuzzyFact>();
                foreach(var value in ClusteringService.CMeans(domain.Clusters.Count(), domainValues))
                    factList.Add(new FuzzyFact(domain, value.Value, value.ClusterDegree));

                fuzzyFacts.Add(domain, factList);
            }
            
            return fuzzyFacts;
        }

        public List<FuzzyDomain> GetFuzzyDomains(List<CustomSocket> sockets)
        {
            var fuzzyDomains = new List<FuzzyDomain>();
            var customSocketType = typeof(CustomSocket);

            foreach (var domain in GetFuzzySocketDomains().Keys)
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