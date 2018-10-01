using ExpertSystem.Models;
using Xunit;

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
            FactSet facts = new FactSet(
                new Fact("NumberOfPositions", 60, typeof(int)),
				new Fact("NumberOfContacts", 120, typeof(int)),
				new Fact("MountingStyle", "Through Hole", typeof(string))
            );

            Assert.True(_logicProcessor.Processing(facts, "5145167-4"));
            Assert.False(_logicProcessor.Processing(facts, "8-215570-0"));
        }
    }
}