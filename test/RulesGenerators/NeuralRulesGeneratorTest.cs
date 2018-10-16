using ExpertSystem.Common.RulesGenerators;
using ExpertSystem.Models;
using Xunit;

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
            var statements = _generator.GetNeuralFuzzyRuleStatements(sockets);
        }
    }
}