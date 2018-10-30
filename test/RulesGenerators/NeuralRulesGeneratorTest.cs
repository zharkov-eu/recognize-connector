using ExpertSystem.Aggregator.Models.ANFIS;
using Xunit;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Tests.Configuration;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class NeuralRulesGeneratorTest
    {
        private readonly NeuralRulesGenerator _generator;
        private NeuralNetwork _network;

        public NeuralRulesGeneratorTest()
        {
            _generator = new NeuralRulesGenerator();
        }

        public NeuralNetwork GetNetwork()
        {
            var sockets = TestData.GetSockets();
            _network = _generator.GetNeuralNetwork(sockets);
            return _network.Learn();
        }

        [Fact]
        public void GetNeuralFuzzyRuleStatements_isCorrect()
        {
            var network = GetNetwork();
        }
    }
}