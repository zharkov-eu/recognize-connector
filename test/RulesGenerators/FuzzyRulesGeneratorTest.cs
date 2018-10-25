using Xunit;
using ExpertSystem.Aggregator.RulesGenerators;
using ExpertSystem.Tests.Parsers;

namespace ExpertSystem.Tests.RulesGenerators
{
    public class FuzzyRulesGeneratorTest
    {
        private readonly FuzzyRulesGenerator _generator;
        private readonly CsvRecordParserTest _recordParser;

        public FuzzyRulesGeneratorTest()
        {
            _generator = new FuzzyRulesGenerator();
            _recordParser = new CsvRecordParserTest();
        }

        [Fact]
        public void GetFuzzyDomains_isCorrect()
        {
            var sockets = _recordParser.GetSockets();
            var domains = _generator.GetFuzzyDomains(sockets);
        }

        [Fact]
        public void GetFuzzyFacts_isCorrect()
        {
            var sockets = _recordParser.GetSockets();
            var domains = _generator.GetFuzzyDomains(sockets);
            var facts = _generator.GetFuzzyFacts(domains, sockets);
        }
    }
}