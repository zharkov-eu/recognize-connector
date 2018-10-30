using ExpertSystem.Aggregator.Models.ANFIS;
using ExpertSystem.Common.Generated;
using ExpertSystem.Tests.RulesGenerators;
using Xunit;
using Xunit.Abstractions;

namespace ExpertSystem.Tests.Processors
{
    public class NeuralProcessorTest
    {
        private readonly ITestOutputHelper _output;
        private readonly NeuralNetwork _network;

        public NeuralProcessorTest(ITestOutputHelper output)
        {
            var generator = new NeuralRulesGeneratorTest();

            _network = generator.GetNetwork();
            _output = output;
        }

        [Fact]
        public void GetNeuralFuzzyRuleStatements_isCorrect()
        {
            var socket = new CustomSocket
            {
                NumberOfContacts = 50,
                SizeLength = 0.03f,
                SizeWidth = 0.0075f
            };
            var result = _network.Process(socket);

            _output.WriteLine($"NeuralProcessing for {socket}: {result}");
            Assert.True(result > 45 && result < 110);
        }
    }
}