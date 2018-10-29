using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Aggregator.Models.FuzzyLogic;
using ExpertSystem.Aggregator.Services;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using ExpertSystem.Common.Utils;

namespace ExpertSystem.Aggregator.RulesGenerators
{
    public class FuzzyRulesGenerator
    {
        public List<FuzzyRuleStatement> GetFuzzyRuleStatements(List<FuzzyDomain> domains)
        {
            var statements = new List<FuzzyRuleStatement>();
            var funcStatements = GetFuzzyFuncStatements(domains);
            var resultFacts = GetAmperageCircuitFact(funcStatements).Distinct().ToDictionary(p => p.Value);

            var defaultSocket = GetDefaultSocket();
            foreach (var statement in funcStatements)
            {
                var fact = resultFacts[statement.Result(defaultSocket)];
                var factCluster = fact.ClusterDegree.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                statements.Add(new FuzzyRuleStatement(
                    statement.Rules, new FuzzyRule(fact.Domain, factCluster).SetFact(fact))
                );
            }

            return statements;
        }

        public List<FuzzyFact> GetAmperageCircuitFact(List<FuzzyFuncStatement> statements)
        {
            var defaultSocket = GetDefaultSocket();
            return GetAmperageCircuitFact(statements, defaultSocket);
        }

        public List<FuzzyFact> GetAmperageCircuitFact(List<FuzzyFuncStatement> statements, CustomSocket socket)
        {
            var values = new List<double>();
            foreach (var statement in statements)
                values.Add(statement.Result(socket));
            var domain = new FuzzyDomain(CustomSocketDomain.SocketDomain.AmperageCircuit, new FuzzyDomainOption
            {
                ClusterCount =
                    FuzzyCustomSocket.GetFuzzySocketDomains()[CustomSocketDomain.SocketDomain.AmperageCircuit],
                Min = values.Min(),
                Max = values.Max()
            });

            var facts = new List<FuzzyFact>();
            foreach (var value in ClusteringService.CMeans(domain.Clusters.Count, values))
                facts.Add(new FuzzyFact(domain, value.Value, value.ClusterDegree));

            return facts;
        }

        public List<FuzzyFuncStatement> GetFuzzyFuncStatements(List<FuzzyDomain> domains)
        {
            var statements = new List<FuzzyFuncStatement>();

            var rulesSets = new List<IEnumerable<FuzzyRule>>();
            foreach (var domain in domains)
                rulesSets.Add(domain.Clusters.Select(cluster => new FuzzyRule(domain, cluster)));

            foreach (var rules in SetOperation.CartesianProduct(rulesSets))
                statements.Add(
                    new FuzzyFuncStatement(
                        new FuzzyRuleSet(rules.ToArray()),
                        FuzzyCustomSocket.GetAmperageCircuitFormula(rules.ToDictionary(p => p.Domain.Domain))
                    )
                );

            return statements;
        }

        public Dictionary<CustomSocketDomain.SocketDomain, List<FuzzyFact>> GetFuzzyFacts(List<FuzzyDomain> domains,
            List<CustomSocket> sockets)
        {
            var socketType = typeof(CustomSocket);
            var fuzzyFacts = new Dictionary<CustomSocketDomain.SocketDomain, List<FuzzyFact>>();

            foreach (var domain in domains)
            {
                var type = CustomSocketDomain.SocketDomainType[domain.Domain];
                var domainValues = sockets
                    .Select(p => socketType.GetProperty(domain.Domain.ToString()).GetValue(p))
                    .Where(p => !CustomSocketDomain.SocketDefaultValue[type].Equals(p))
                    .Distinct()
                    .Select(p => Convert.ToDouble(p))
                    .ToList();

                var factList = new List<FuzzyFact>();

                foreach (var value in ClusteringService.CMeans(domain.Clusters.Count(), domainValues))
                    factList.Add(new FuzzyFact(domain, value.Value, value.ClusterDegree));

                fuzzyFacts.Add(domain.Domain, SortClusters(factList));
            }

            return fuzzyFacts;
        }

        public List<FuzzyDomain> GetFuzzyDomains(List<CustomSocket> sockets)
        {
            var socketType = typeof(CustomSocket);
            var fuzzyDomains = new List<FuzzyDomain>();
            var fuzzySocketDomain = FuzzyCustomSocket.GetFuzzySocketDomains();

            foreach (var domain in fuzzySocketDomain.Keys.Where(p => !CustomSocketDomain.DomainIgnore.Contains(p)))
            {
                var type = CustomSocketDomain.SocketDomainType[domain];
                var domainValues = sockets.Select(p => socketType.GetProperty(domain.ToString()).GetValue(p))
                    .Where(p => !CustomSocketDomain.SocketDefaultValue[type].Equals(p));

                fuzzyDomains.Add(new FuzzyDomain(
                    domain, new FuzzyDomainOption
                    {
                        ClusterCount = fuzzySocketDomain[domain],
                        Min = domainValues.Min(),
                        Max = domainValues.Max()
                    }
                ));
            }

            return fuzzyDomains;
        }

        private List<FuzzyFact> SortClusters(List<FuzzyFact> facts)
        {
            if (facts.Count == 0)
                return facts;

            var domain = facts.FirstOrDefault().Domain;

            var factClusters = new Dictionary<int, List<double>>();
            var sortedClusters = new int[domain.Clusters.Count];
            for (var i = 0; i < domain.Clusters.Count; i++)
            {
                factClusters.Add(i, new List<double>());
                sortedClusters[i] = i;
            }

            foreach (var fact in facts)
                factClusters[fact.GetMostProbableCluster()].Add(fact.Value);
            sortedClusters = sortedClusters
                .OrderBy(id => factClusters[id].Count == 0 ? -1 : factClusters[id].Average())
                .ToArray();

            foreach (var fact in facts)
            {
                var clusterDegree = new Dictionary<int, double>();
                for (var i = 0; i < sortedClusters.Count(); i++)
                    clusterDegree[i] = fact.ClusterDegree[sortedClusters[i]];
                fact.ClusterDegree = clusterDegree;
            }

            return facts;
        }

        private static CustomSocket GetDefaultSocket()
        {
            return new CustomSocket
            {
                NumberOfContacts = 50,
                SizeLength = 0.03f,
                SizeWidth = 0.0075f
            };
        }
    }
}