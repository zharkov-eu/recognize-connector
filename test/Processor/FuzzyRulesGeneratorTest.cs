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
            Dictionary<FuzzyDomain, List<FuzzyFact>> facts = _generator.GetFuzzyFacts(domains, sockets);
        }

        [Fact]
		public void FactFuzzification_isCorrect()
        {
            var sockets = _socketFieldsProcessor.GetSockets();
            List<FuzzyDomain> domains = _generator.GetFuzzyDomains(sockets);
            Dictionary<FuzzyDomain, List<FuzzyFact>> facts = _generator.GetFuzzyFacts(domains, sockets);

            Fact fact = new Fact(SocketDomain.NumberOfContacts, 81);
            FuzzyDomain domain = domains.Where(p => p.Domain.Equals(SocketDomain.NumberOfContacts)).First();
            SortedList<double, FuzzyFact> domainFacts = new SortedList<double, FuzzyFact>(facts[domain].ToDictionary(s => (double) s.Value));

            FuzzyFact fuzzificatedFact = _generator.FactFuzzification(fact, domainFacts);
        }
    }
}