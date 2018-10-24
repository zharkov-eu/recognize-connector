using ExpertSystem.Aggregator.RulesGenerators;
using Xunit;
using ExpertSystem.Tests.Parsers;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class NeuralRulesGeneratorTest
    {
        private readonly NeuralRulesGenerator _generator;
        private readonly CustomParserTest _customParser;

        public NeuralRulesGeneratorTest()
        {
            _generator = new NeuralRulesGenerator();
            _customParser = new CustomParserTest();
        }

        [Fact]
        public void GetNeuralFuzzyRuleStatements_isCorrect()
        {
            var sockets = _customParser.GetSockets();
            var network = _generator.GetNeuralNetwork(sockets);
        }
    }
}