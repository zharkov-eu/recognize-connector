using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Aggregator.Models.ANFIS;
using ExpertSystem.Aggregator.Models.FuzzyLogic;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Aggregator.RulesGenerators
{
    public class NeuralRulesGenerator
    {
        private readonly FuzzyRulesGenerator _generator;
        private readonly Dictionary<CustomSocket, double> _samples;

        public NeuralRulesGenerator()
        {
            _generator = new FuzzyRulesGenerator();
            _samples = new Dictionary<CustomSocket, double>();
        }

        public NeuralNetwork GetNeuralNetwork(List<CustomSocket> sockets)
        {
            var domains = _generator.GetFuzzyDomains(sockets);
            var funcStatements = _generator.GetFuzzyFuncStatements(domains);

            foreach (var socket in sockets)
                _samples.Add(socket, funcStatements[0].Result(socket));

            return new NeuralNetwork().Initialize(funcStatements.Cast<FuzzyStatement>().ToList());
        }

        public NeuralNetwork LearnNeuralNetwork(NeuralNetwork network)
        {
            return network.Learn(_samples);
        }
    }
}