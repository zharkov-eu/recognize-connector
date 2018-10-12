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
        public List<FuzzyRuleStatement> GetFuzzyRuleStatements(List<FuzzyDomain> domains)
        {
            var statements = new List<FuzzyRuleStatement>();
            var resultFacts = GetAmperageCircuitFact(GetFuzzyFuncStatements(domains));

            return statements;
        }

        public List<FuzzyFact> GetAmperageCircuitFact(List<FuzzyFuncStatement> statements)
        {
            CustomSocket defaultSocket = new CustomSocket();
            defaultSocket.NumberOfContacts = 50;
            defaultSocket.SizeLength = 0.03f;
            defaultSocket.SizeWidth = 0.0075f;

            var values = new List<double>();
            foreach(var statement in statements)
                values.Add(statement.Result(defaultSocket));
            FuzzyDomain domain = new FuzzyDomain(SocketDomain.AmperageCircuit, new FuzzyDomainOption {
                ClusterCount = GetFuzzySocketDomains()[SocketDomain.AmperageCircuit],
                Min = values.Min(),
                Max = values.Max()
            }); 

            var facts = new List<FuzzyFact>();
            foreach(var value in ClusteringService.CMeans(domain.Clusters.Count, values))
                facts.Add(new FuzzyFact(domain, value.Value, value.ClusterDegree));

            return facts;
        }

        public List<FuzzyFuncStatement> GetFuzzyFuncStatements(List<FuzzyDomain> domains)
        {
            var statements = new List<FuzzyFuncStatement>();

            var rulesSets = new List<IEnumerable<FuzzyRule>>();
            foreach (var domain in domains)
                rulesSets.Add(domain.Clusters.Select(cluster => new FuzzyRule(domain, cluster)));

            foreach (var rules in SetOperation.CartesianProduct<FuzzyRule>(rulesSets))
                statements.Add(
                    new FuzzyFuncStatement(rules.ToHashSet(), GetAmperageCircuitFormula(rules.ToDictionary(p => p.Domain.Domain)))
                );
            
            return statements;
        }

        public Dictionary<SocketDomain, List<FuzzyFact>> GetFuzzyFacts(List<FuzzyDomain> domains, List<CustomSocket> sockets)
        {
            var fuzzyFacts = new Dictionary<SocketDomain, List<FuzzyFact>>();

            foreach (var domain in domains)
            {
                Type type = SocketDomainType[domain.Domain];
                List<double> domainValues = sockets
                    .Select(p => CustomSocket.Type.GetField(domain.Domain.ToString()).GetValue(p))
                    .Where(p => !SocketDefaultValue[type].Equals(p))
                    .Distinct()
                    .Select(p => Convert.ToDouble(p))
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
            var fuzzySocketDomain = GetFuzzySocketDomains();

            foreach (var domain in fuzzySocketDomain.Keys)
            {
                Type type = SocketDomainType[domain];
                var domainValues = sockets.Select(p => CustomSocket.Type.GetField(domain.ToString()).GetValue(p))
                    .Where(p => !SocketDefaultValue[type].Equals(p));

                fuzzyDomains.Add(new FuzzyDomain(
                    domain, new FuzzyDomainOption {
                        ClusterCount = fuzzySocketDomain[domain],
                        Min = domainValues.Min(),
                        Max = domainValues.Max()
                    }
                ));
            }

            return fuzzyDomains;
        }
    }
}