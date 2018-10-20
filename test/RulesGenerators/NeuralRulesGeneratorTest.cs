using Xunit;
using ExpertSystem.Client.Models;
using ExpertSystem.Client.RulesGenerators;
using ExpertSystem.Tests.Parsers;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class NeuralRulesGeneratorTest
    {
        private readonly NeuralRulesGenerator _generator;
        private readonly SocketFieldsProcessorTest _socketFieldsProcessor;

        public NeuralRulesGeneratorTest()
        {
            _generator = new NeuralRulesGenerator();
            _socketFieldsProcessor = new SocketFieldsProcessorTest();
        }

        [Fact]
        public void GetNeuralFuzzyRuleStatements_isCorrect()
        {
            var sockets = _socketFieldsProcessor.GetSockets();
            var network = _generator.GetNeuralNetwork(sockets);
        }
    }
}