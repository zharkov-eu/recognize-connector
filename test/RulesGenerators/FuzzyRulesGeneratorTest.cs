using ExpertSystem.Aggregator.RulesGenerators;
using Xunit;
using ExpertSystem.Tests.Parsers;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class FuzzyRulesGeneratorTest
    {
        private readonly FuzzyRulesGenerator _generator;
        private readonly SocketParserTest _socketParser;

        public FuzzyRulesGeneratorTest()
        {
            _generator = new FuzzyRulesGenerator();
            _socketParser = new SocketParserTest();
        }

        [Fact]
        public void GetFuzzyDomains_isCorrect()
        {
            var sockets = _socketParser.GetSockets();
            var domains = _generator.GetFuzzyDomains(sockets);
        }

        [Fact]
        public void GetFuzzyFacts_isCorrect()
        {
            var sockets = _socketParser.GetSockets();
            var domains = _generator.GetFuzzyDomains(sockets);
            var facts = _generator.GetFuzzyFacts(domains, sockets);
        }
    }
}