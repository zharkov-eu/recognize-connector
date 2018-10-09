using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Services;
using ExpertSystem.Utils;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor
{
    public class FuzzyRulesGenerator
    {
        public List<FuzzyStatement> GetFuzzyStatements(List<FuzzyDomain> domains)
        {
            var statements = new List<FuzzyStatement>();

            var rulesSets = new List<IEnumerable<FuzzyRule>>();
            foreach (var domain in domains)
                rulesSets.Add(domain.Clusters.Select(cluster => new FuzzyRule(domain, cluster)));

            foreach (var rules in SetOperation.CartesianProduct<FuzzyRule>(rulesSets))
                statements.Add(
                    new FuzzyStatement(rules.ToHashSet(), GetPinPitchFormula(rules.ToDictionary(p => p.Domain.Domain)))
                );
            
            return statements;
        }

        public Dictionary<SocketDomain, List<FuzzyFact>> GetFuzzyFacts(List<FuzzyDomain> domains, List<CustomSocket> sockets)
        {
            var fuzzyFacts = new Dictionary<SocketDomain, List<FuzzyFact>>();
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

                fuzzyFacts.Add(domain.Domain, factList);
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