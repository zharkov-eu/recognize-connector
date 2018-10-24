using ExpertSystem.Aggregator.RulesGenerators;
using Xunit;
using ExpertSystem.Tests.Parsers;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class NeuralRulesGeneratorTest
    {
        private readonly NeuralRulesGenerator _generator;
        private readonly SocketParserTest _socketParser;

        public NeuralRulesGeneratorTest()
        {
            _generator = new NeuralRulesGenerator();
            _socketParser = new SocketParserTest();
        }

        [Fact]
        public void GetNeuralFuzzyRuleStatements_isCorrect()
        {
            var sockets = _socketParser.GetSockets();
            var network = _generator.GetNeuralNetwork(sockets);
        }
    }
}