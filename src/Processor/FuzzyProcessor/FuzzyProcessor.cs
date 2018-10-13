using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor.FuzzyProcessor
{
    public class FuzzyProcessor : AbstractProcessor
    {
        private readonly FuzzyRulesGenerator _generator;
        private readonly List<FuzzyDomain> _domains;
        private readonly Dictionary<SocketDomain, List<FuzzyFact>> _domainFacts;

        public FuzzyProcessor(List<FuzzyDomain> domains,
                              Dictionary<SocketDomain, List<FuzzyFact>> domainFacts,
                              ProcessorOptions options)
            : base(options)
        {
            _generator = new FuzzyRulesGenerator();
            _domains = domains;
            _domainFacts = domainFacts;
        }
        
        public double MamdaniProcesing(FactSet factSet)
        {
            var fuzzyFacts = new List<FuzzyFact>();
            foreach (var fact in factSet.Facts)
                fuzzyFacts.Add(FactFuzzification(fact));

            List<FuzzyFact> resultFacts = _generator.GetAmperageCircuitFact(_generator.GetFuzzyFuncStatements(_domains));
            Dictionary<object, double> resultDegrees = new Dictionary<object, double>();
            foreach (var result in resultFacts)
                resultDegrees.Add(result.Value, 0);

            List<FuzzyRuleStatement> statements = _generator.GetFuzzyRuleStatements(_domains);


            foreach (var statement in statements)
            {
                var degree = statement.SetRulesFacts(fuzzyFacts).GetRulesDegree();
                foreach (var result in resultFacts)
                {
                    var resultDegree = result.ClusterDegree[statement.Result.Cluster];
                    if (resultDegree > degree)
                        resultDegree = degree;
                    
                    if (resultDegrees[result.Value] < resultDegree)
                        resultDegrees[result.Value] = resultDegree;
                }
            }

            var numerator = resultDegrees.Aggregate(0d, (acc, p) => acc + Convert.ToDouble(p.Key) * p.Value);
            var denumerator = resultDegrees.Aggregate(0d, (acc, p) => acc + p.Value);

            return numerator / denumerator;
        }

        public double SugenoProcesing(FactSet factSet)
        {
            var socket = new CustomSocket();
            var fuzzyFacts = new List<FuzzyFact>();
            foreach (var fact in factSet.Facts)
            {
                CustomSocket.Type.GetField(fact.Domain.ToString()).SetValue(socket, fact.Value);
                fuzzyFacts.Add(FactFuzzification(fact));
            }

            var statements = _generator.GetFuzzyFuncStatements(_domains);

            var degreeResults = new List<FuzzyFuncProcessed>();
            foreach (var statement in statements)
            {   
                var degree = statement.SetRulesFacts(fuzzyFacts).GetRulesDegree();
                var result = statement.Result(socket);
                degreeResults.Add(new FuzzyFuncProcessed { Degree = degree, Result = result });
            }

            var numerator = degreeResults.Aggregate(0d, (acc, p) => acc + p.Degree * p.Result);
            var denumerator = degreeResults.Aggregate(0d, (acc, p) => acc + p.Degree);

            return numerator / denumerator;
        }

        public FuzzyFact FactFuzzification(Fact fact)
        {
            var currentFacts = new SortedList<double, FuzzyFact>(
                _domainFacts[fact.Domain].ToDictionary(p => (double) p.Value)
            );
            return FactFuzzification(fact, currentFacts);
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
            double factValue = Convert.ToDouble(fact.Value);
            double currentMinValue = currentFacts.Keys.Min();
            double currentMaxValue = currentFacts.Keys.Max();
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
                double difference = factValue - Convert.ToDouble(currentFact.Value);
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
            double offset = Math.Abs(left.Difference) / (Math.Abs(left.Difference) + Math.Abs(right.Difference));
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