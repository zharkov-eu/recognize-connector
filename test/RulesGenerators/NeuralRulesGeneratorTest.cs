using Xunit;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Tests.Configuration;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class NeuralRulesGeneratorTest
    {
        private readonly NeuralRulesGenerator _generator;

        public NeuralRulesGeneratorTest()
        {
            _generator = new NeuralRulesGenerator();
        }

        [Fact]
        public void GetNeuralFuzzyRuleStatements_isCorrect()
        {
            var sockets = TestData.GetSockets();
            var network = _generator.GetNeuralNetwork(sockets);
        }
    }
}