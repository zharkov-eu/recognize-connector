using Xunit;
using ExpertSystem.Models;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor.LogicProcessor
{
    public class LogicProcessorTest
    {
        private readonly LogicProcessor _logicProcessor;

        public LogicProcessorTest()
        {
            var socketFieldsProcessor = new SocketFieldsProcessorTest();
            var sockets = socketFieldsProcessor.GetSockets();

            var rulesGenerator = new LogicRulesGenerator();
            var logicRules = rulesGenerator.GenerateRules(sockets);

            _logicProcessor = new LogicProcessor(logicRules, new ProcessorOptions { Debug = true });
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
            Assert.False(_logicProcessor.Processing(facts, "8-215570-0"));
        }
    }
}