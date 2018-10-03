using Xunit;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor
{
	public class FuzzyRulesGeneratorTest
	{
        private FuzzyRulesGenerator _generator;

        public FuzzyRulesGeneratorTest()
        {
            _generator = new FuzzyRulesGenerator();
        }

        [Fact]
		public void GetFuzzyDomains_isCorrect()
        {
            var socketFieldsProcessor = new SocketFieldsProcessorTest();
			var sockets = socketFieldsProcessor.GetSockets();
            _generator.GetFuzzyDomains(sockets);
        }
    }
}