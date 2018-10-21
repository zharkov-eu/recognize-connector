using Xunit;
using Xunit.Abstractions;
using ExpertSystem.Client.Models;
using ExpertSystem.Client.Processors;
using ExpertSystem.Client.RulesGenerators;
using ExpertSystem.Tests.Parsers;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Tests.Processors
{
    public class FuzzyRulesProcessorTest
    {
        private readonly ITestOutputHelper _output;
        private readonly FuzzyProcessor _processor;

        public FuzzyRulesProcessorTest(ITestOutputHelper outputHelper)
        {
            var generator = new FuzzyRulesGenerator();
            var socketParser = new SocketParserTest();

            var sockets = socketParser.GetSockets();
            var fuzzyDomains = generator.GetFuzzyDomains(sockets);
            var fuzzyFacts = generator.GetFuzzyFacts(fuzzyDomains, sockets);

            _output = outputHelper;
            _processor = new FuzzyProcessor(fuzzyDomains, fuzzyFacts, new ProcessorOptions { Debug = false });
        }

        [Fact]
        public void FactFuzzification_isCorrect()
        {
            var fact = new Fact(SocketDomain.NumberOfContacts, 81);
            var fuzzyFact = _processor.FactFuzzification(fact);
        }

        [Fact]
        public void MamdaniProcesing_IsCorrect()
        {
            var factSet = new FactSet(
                new Fact(SocketDomain.NumberOfContacts, 50),
                new Fact(SocketDomain.SizeLength, 0.03f),
                new Fact(SocketDomain.SizeWidth, 0.0075f)
            );
            var result = _processor.MamdaniProcessing(factSet);

            _output.WriteLine($"MamdaniProcessing for {factSet}: {result}");
            Assert.True(result > 45 && result < 110);
        }

        [Fact]
        public void SugenoProcesing_IsCorrect()
        {
            var factSet = new FactSet(
                new Fact(SocketDomain.NumberOfContacts, 50),
                new Fact(SocketDomain.SizeLength, 0.03f),
                new Fact(SocketDomain.SizeWidth, 0.0075f)
            );
            var result = _processor.SugenoProcessing(factSet);

            _output.WriteLine($"SugenoProcessing for {factSet}: {result}");
            Assert.True(result > 45 && result < 110);
        }
    }
}