using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Client.Models.ANFIS;
using ExpertSystem.Client.Models.FuzzyLogic;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Client.RulesGenerators
{
    public class NeuralRulesGenerator
    {
        private readonly FuzzyRulesGenerator _generator;

        public NeuralRulesGenerator()
        {
            _generator = new FuzzyRulesGenerator();
        }

        public NeuralNetwork GetNeuralNetwork(List<CustomSocket> sockets)
        {
            var network = new NeuralNetwork();
            var domains = _generator.GetFuzzyDomains(sockets);
            var funcStatements = _generator.GetFuzzyFuncStatements(domains);

            // Инициализация нейросети
            network.Initialize(funcStatements.Cast<FuzzyStatement>().ToList());

            // Обучение
            var samples = new Dictionary<CustomSocket, double>();
            foreach (var socket in sockets)
                samples.Add(socket, funcStatements[0].Result(socket));
            network.Learn(samples);

            return network;
        }
    }
}