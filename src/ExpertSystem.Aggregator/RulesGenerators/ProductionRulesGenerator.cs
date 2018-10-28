using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Aggregator.Models.Graph;
using ExpertSystem.Common.Generated;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Aggregator.RulesGenerators
{
    public class ProductionRulesGenerator
    {
        public RulesGraph GenerateRules(List<CustomSocket> sockets)
        {
            var rulesGraph = new RulesGraph(GetSocketDomainsOrdered(sockets));

            foreach (var socket in sockets)
                rulesGraph.AddSocketRules(socket);

            return rulesGraph.Compress();
        }

        private static List<SocketDomain> GetSocketDomainsOrdered(IReadOnlyCollection<CustomSocket> sockets)
        {
            var domainValues = GetDomainsWithPossibleValues(sockets);
            domainValues = domainValues.OrderBy(p => p.Value.Count).ToDictionary(x => x.Key, x => x.Value);
            return domainValues.Keys.ToList();
        }

        private static Dictionary<SocketDomain, List<string>> GetDomainsWithPossibleValues(
            IReadOnlyCollection<CustomSocket> sockets)
        {
            var type = typeof(CustomSocket);
            var domainsValues = new Dictionary<SocketDomain, List<string>>();
            foreach (var domain in GetSocketDomains()
                .Where(p => p != SocketDomain.SocketName))
            {
                var field = type.GetProperty(domain.ToString());

                var propertyValues = sockets.GroupBy(p => field.GetValue(p).ToString()).ToList();
                var currentPropValues = new List<string>();
                foreach (var value in propertyValues)
                    currentPropValues.Add(value.Key);

                domainsValues.Add(domain, currentPropValues);
            }

            return domainsValues;
        }
    }
}