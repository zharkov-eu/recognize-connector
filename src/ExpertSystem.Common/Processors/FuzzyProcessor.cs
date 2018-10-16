using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.RulesGenerators;
using ExpertSystem.Models;
using ExpertSystem.Models.FuzzyLogic;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Common.Processors
{
    public class FuzzyProcessor : AbstractProcessor
    {
        private readonly Dictionary<SocketDomain, List<FuzzyFact>> _domainFacts;
        private readonly List<FuzzyDomain> _domains;
        private readonly FuzzyRulesGenerator _generator;

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
            debug($"Приведение к нечеткости входных фактов:\n" + string.Join("\n", fuzzyFacts));

            var resultFacts = _generator.GetAmperageCircuitFact(_generator.GetFuzzyFuncStatements(_domains));
            var resultDegrees = new Dictionary<double, double>();
            foreach (var result in resultFacts)
            {
                if (resultDegrees.ContainsKey(result.Value))
                    continue;
                resultDegrees.Add(result.Value, 0);
            }

            var statements = _generator.GetFuzzyRuleStatements(_domains);

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

            debug($"Нечеткие правила:\n" + string.Join("\n", statements));

            var numerator = resultDegrees.Aggregate(0d, (acc, p) => acc + p.Key * p.Value);
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

            debug($"Приведение к нечеткости входных фактов:\n" + string.Join("\n", fuzzyFacts));

            var statements = _generator.GetFuzzyFuncStatements(_domains);

            var degreeResults = new List<FuzzyFuncProcessed>();
            foreach (var statement in statements)
            {
                var degree = statement.SetRulesFacts(fuzzyFacts).GetRulesDegree();
                var result = statement.Result(socket);
                degreeResults.Add(new FuzzyFuncProcessed {Degree = degree, Result = result});
            }

            debug($"Нечеткие правила:\n" + string.Join("\n", statements));

            var numerator = degreeResults.Aggregate(0d, (acc, p) => acc + p.Degree * p.Result);
            var denumerator = degreeResults.Aggregate(0d, (acc, p) => acc + p.Degree);

            return numerator / denumerator;
        }

        public FuzzyFact FactFuzzification(Fact fact)
        {
            var currentFacts = new SortedList<double, FuzzyFact>(
                _domainFacts[fact.Domain].ToDictionary(p => p.Value)
            );
            return FactFuzzification(fact, currentFacts);
        }

        public FuzzyFact FactFuzzification(Fact fact, SortedList<double, FuzzyFact> currentFacts)
        {
            // Проверяем текущий лист правил
            if (currentFacts.Values.Count == 0)
                throw new Exception("FactFuzzification: base List<FuzzyFact> is empty");

            // Проверяем эквивалентность доменов
            var domain = currentFacts.First().Value.Domain;
            if (!fact.Domain.Equals(domain.Domain))
                throw new Exception("FactFuzzification: domains not equal");

            // Проверяем 10% отклонение от минимального и максимального значений
            var factValue = Convert.ToDouble(fact.Value);
            var currentMinValue = currentFacts.Keys.Min();
            var currentMaxValue = currentFacts.Keys.Max();
            var currentDifference = currentMaxValue - currentMinValue;
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
            var left = default(FuzzificationFact);
            var right = default(FuzzificationFact);
            foreach (var currentFact in currentFacts.Values)
            {
                if (currentFact.Value.Equals(fact.Value))
                    return currentFact;
                var difference = factValue - currentFact.Value;
                if (difference > 0 && (left.Equals(default(FuzzificationFact)) || difference < left.Difference))
                    left = new FuzzificationFact {Fact = currentFact, Difference = difference};
                else if (difference < 0 && (right.Equals(default(FuzzificationFact)) ||
                                            Math.Abs(difference) < Math.Abs(right.Difference)))
                    right = new FuzzificationFact {Fact = currentFact, Difference = difference};
            }

            // Вычисляем итоговую принадлежность к кластерам
            var clusterDegree = new Dictionary<int, double>();
            var offset = Math.Abs(left.Difference) / (Math.Abs(left.Difference) + Math.Abs(right.Difference));
            foreach (var key in left.Fact.ClusterDegree.Keys)
            {
                var leftDegree = left.Fact.ClusterDegree[key];
                var rightDegree = right.Fact.ClusterDegree[key];
                var degreeOffset = Math.Abs(leftDegree - rightDegree) * offset;
                var resultDegree = leftDegree + (leftDegree < rightDegree ? degreeOffset : -degreeOffset);
                clusterDegree.Add(key, resultDegree);
            }

            return new FuzzyFact(domain, Convert.ToDouble(fact.Value), clusterDegree);
        }

        private struct FuzzificationFact
        {
            public FuzzyFact Fact;
            public double Difference;
        }
    }
}