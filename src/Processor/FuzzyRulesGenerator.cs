using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor
{
    public class FuzzyRulesGenerator
    {
        public FuzzyFact FactFuzzification(Fact fact, SortedList<double, FuzzyFact> currentFacts)
        {
            (fact.Type) fact.Value
            if (currentFacts.Values.Count == 0)
                throw new Exception("FactFuzzification: base SortedList<FuzzyFact> is empty");
            FuzzyFact left = currentFacts.Values[0];
            FuzzyFact right = currentFacts.Values[0];
            foreach (var currentFact in currentFacts.Values)
            {
                if (currentFact.Value.Equals(fact.Value))
                    return currentFact;
                double difference = (double) fact.Value - (double) currentFact.Value;
            }
            var fuzzyFact = new FuzzyFact();
            return fuzzyFactSet;
        }

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