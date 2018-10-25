using Xunit;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Tests.Parsers;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class NeuralRulesGeneratorTest
    {
        private readonly NeuralRulesGenerator _generator;
        private readonly CsvRecordParserTest _recordParser;

        public NeuralRulesGeneratorTest()
        {
            _generator = new NeuralRulesGenerator();
            _recordParser = new CsvRecordParserTest();
        }

        [Fact]
        public void GetNeuralFuzzyRuleStatements_isCorrect()
        {
            var sockets = _recordParser.GetSockets();
            var network = _generator.GetNeuralNetwork(sockets);
        }
    }
}