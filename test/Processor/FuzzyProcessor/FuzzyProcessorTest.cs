using Xunit;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor.FuzzyProcessor
{
	public class FuzzyRulesProcessorTest
	{
        private FuzzyProcessor _processor;

        public FuzzyRulesProcessorTest()
        {
            var generator = new FuzzyRulesGenerator();
            var socketFieldsProcessor = new SocketFieldsProcessorTest();

            var sockets = socketFieldsProcessor.GetSockets();
            var fuzzyDomains = generator.GetFuzzyDomains(sockets);
            var fuzzyFacts = generator.GetFuzzyFacts(fuzzyDomains, sockets);

            _processor = new FuzzyProcessor(fuzzyDomains, fuzzyFacts, new ProcessorOptions { Debug = false });
        }

        [Fact]
		public void FactFuzzification_isCorrect()
        {
            Fact fact = new Fact(SocketDomain.NumberOfContacts, 81);
            FuzzyFact fuzzyFact = _processor.FactFuzzification(fact);
        }
    }
}