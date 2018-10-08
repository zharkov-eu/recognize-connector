using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor.FuzzyProcessor
{
    public class FuzzyProcessor : AbstractProcessor
    {
        private readonly Dictionary<SocketDomain, List<FuzzyFact>> _domainFacts;

        public FuzzyProcessor(Dictionary<SocketDomain, List<FuzzyFact>> domainFacts, ProcessorOptions options)
            : base(options)
        {
            _domainFacts = domainFacts;
        }
        
        public void Procesing(FactSet factSet)
        {
            List<FuzzyFact> fuzzyFacts = new List<FuzzyFact>();
            foreach (var fact in factSet.Facts)
            {
                var currentFacts = new SortedList<double, FuzzyFact>(_domainFacts[fact.Domain].ToDictionary(p => (double) p.Value));
                fuzzyFacts.Add(FactFuzzification(fact, currentFacts));
            }
        }

        public FuzzyFact FactFuzzification(Fact fact, SortedList<double, FuzzyFact> currentFacts)
        {
            // Проверяем текущий лист правил
            if (currentFacts.Values.Count == 0)
                throw new Exception("FactFuzzification: base List<FuzzyFact> is empty");

            // Проверяем эквивалентность доменов
            FuzzyDomain domain = currentFacts.First().Value.Domain;
            if (!fact.Domain.Equals(domain.Domain))
                throw new Exception("FactFuzzification: domains not equal");

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

        private struct FuzzificationFact
        {
            public FuzzyFact Fact;
            public double Difference;
        }
    }
}