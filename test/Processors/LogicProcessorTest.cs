using Xunit;
using ExpertSystem.Client.Models;
using ExpertSystem.Client.Processors;
using ExpertSystem.Client.RulesGenerators;
using ExpertSystem.Tests.Parsers;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Tests.Processors
{
    public class LogicProcessorTest
    {
        private readonly LogicProcessor _logicProcessor;

        public LogicProcessorTest()
        {
            var socketParser = new CustomParserTest();
            var sockets = socketParser.GetSockets();

            var rulesGenerator = new LogicRulesGenerator();
            var logicRules = rulesGenerator.GenerateRules(sockets);

            _logicProcessor = new LogicProcessor(logicRules, new ProcessorOptions { Debug = false });
        }

        [Fact]
        public void Processing_IsCorrect()
        {
            var facts = new FactSet(
                new Fact(SocketDomain.NumberOfPositions, 60),
                new Fact(SocketDomain.NumberOfContacts, 120),
                new Fact(SocketDomain.MountingStyle, "Through Hole")
            );

            Assert.True(_logicProcessor.Processing(facts, "5145167-4"));
            Assert.False(_logicProcessor.Processing(facts, "3-644540-7"));
        }
    }
}