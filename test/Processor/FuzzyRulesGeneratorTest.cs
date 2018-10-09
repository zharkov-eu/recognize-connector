using Xunit;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor
{
	public class FuzzyRulesGeneratorTest
	{
        private FuzzyRulesGenerator _generator;
        private SocketFieldsProcessorTest _socketFieldsProcessor;

        public FuzzyRulesGeneratorTest()
        {
            _generator = new FuzzyRulesGenerator();
            _socketFieldsProcessor = new SocketFieldsProcessorTest();
        }

        [Fact]
		public void GetFuzzyDomains_isCorrect()
        {
			var sockets = _socketFieldsProcessor.GetSockets();
            List<FuzzyDomain> domains = _generator.GetFuzzyDomains(sockets);
        }

        [Fact]
		public void GetFuzzyFacts_isCorrect()
        {
			var sockets = _socketFieldsProcessor.GetSockets();
            List<FuzzyDomain> domains = _generator.GetFuzzyDomains(sockets);
            Dictionary<SocketDomain, List<FuzzyFact>> facts = _generator.GetFuzzyFacts(domains, sockets);
        }
    }
}