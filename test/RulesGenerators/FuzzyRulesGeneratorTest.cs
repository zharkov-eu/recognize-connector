using Xunit;
using ExpertSystem.Client.Models;
using ExpertSystem.Client.RulesGenerators;
using ExpertSystem.Tests.Parsers;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class FuzzyRulesGeneratorTest
    {
        private readonly FuzzyRulesGenerator _generator;
        private readonly SocketFieldsProcessorTest _socketFieldsProcessor;

        public FuzzyRulesGeneratorTest()
        {
            _generator = new FuzzyRulesGenerator();
            _socketFieldsProcessor = new SocketFieldsProcessorTest();
        }

        [Fact]
        public void GetFuzzyDomains_isCorrect()
        {
            var sockets = _socketFieldsProcessor.GetSockets();
            var domains = _generator.GetFuzzyDomains(sockets);
        }

        [Fact]
        public void GetFuzzyFacts_isCorrect()
        {
            var sockets = _socketFieldsProcessor.GetSockets();
            var domains = _generator.GetFuzzyDomains(sockets);
            var facts = _generator.GetFuzzyFacts(domains, sockets);
        }
    }
}