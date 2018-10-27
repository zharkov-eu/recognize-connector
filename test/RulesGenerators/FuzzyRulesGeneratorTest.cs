using Xunit;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Tests.Configuration;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class FuzzyRulesGeneratorTest
    {
        private readonly FuzzyRulesGenerator _generator;

        public FuzzyRulesGeneratorTest()
        {
            _generator = new FuzzyRulesGenerator();
        }

        [Fact]
        public void GetFuzzyDomains_isCorrect()
        {
            var sockets = TestData.GetSockets();
            var domains = _generator.GetFuzzyDomains(sockets);
        }

        [Fact]
        public void GetFuzzyFacts_isCorrect()
        {
            var sockets = TestData.GetSockets();
            var domains = _generator.GetFuzzyDomains(sockets);
            var facts = _generator.GetFuzzyFacts(domains, sockets);
        }
    }
}