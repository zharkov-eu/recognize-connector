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
        private struct FuzzificationFact
        {
            public FuzzyFact Fact;
            public double Difference;
        }

        public FuzzyFact FactFuzzification(FuzzyDomain domain, Fact fact, SortedList<double, FuzzyFact> currentFacts)
        {
            // Проверяем текущий лист правил
            if (currentFacts.Values.Count == 0)
                throw new Exception("FactFuzzification: base List<FuzzyFact> is empty");
            // Проверяем 10% отклонение от минимального и максимального значений
            double factValue = (double) fact.Value;
            double currentMinValue = currentFacts.Min().Key;
            double currentMaxValue = currentFacts.Max().Key;
            double currentDifference = currentMaxValue - currentMinValue;
            if (factValue < currentMinValue && Math.Abs(factValue - currentMinValue) > 0.1 * currentDifference)
                throw new Exception("FactFuzzification: fact value less than Min(CurrentFacts) more than 10%");
            if (factValue > currentMaxValue && Math.Abs(factValue - currentMaxValue) > 0.1 * currentDifference)
                throw new Exception("FactFuzzification: fact value less than Max(CurrentFacts) more than 10%");

            // Проверяем на выход за область определения имеющихся фактов
            if (factValue < currentMinValue)
                return currentFacts.First().Value;
            if (factValue > currentMaxValue)
                return currentFacts.Last().Value;

            // Находим ближайшее левое и правое значение
            FuzzificationFact left = default(FuzzificationFact);
            FuzzificationFact right = default(FuzzificationFact);
            foreach (var currentFact in currentFacts.Values)
            {
                if (currentFact.Value.Equals(fact.Value))
                    return currentFact;
                double difference = factValue - (double) currentFact.Value;
                if (difference > 0 && (left.Equals(default(FuzzificationFact)) || difference < left.Difference))
                {
                    left = new FuzzificationFact { Fact = currentFact, Difference = difference };
                }
                else if (difference < 0 && (right.Equals(default(FuzzificationFact)) || Math.Abs(difference) < Math.Abs(right.Difference)))
                {
                    right = new FuzzificationFact { Fact = currentFact, Difference = difference };
                }
            }

            // Вычисляем итоговую принадлежность к кластерам
            Dictionary<int, double> clusterDegree = new Dictionary<int, double>();
            double offset = Math.Abs(left.Difference) / Math.Abs(left.Difference + right.Difference);
            foreach (var key in left.Fact.ClusterDegree.Keys)
            {
                var leftDegree = left.Fact.ClusterDegree[key];
                var rightDegree = right.Fact.ClusterDegree[key];
                var degreeOffset = Math.Abs(leftDegree - rightDegree) * offset;
                var resultDegree = leftDegree + (leftDegree < rightDegree ? degreeOffset : -degreeOffset);
                clusterDegree.Add(key, resultDegree);
            }
            
            return new FuzzyFact(domain, fact.Value, clusterDegree);
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