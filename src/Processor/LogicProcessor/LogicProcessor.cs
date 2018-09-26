using ExpertSystem.Models.Graph;

namespace ExpertSystem.Processor
{
    public class LogicProcessor : AbstractProcessor
    {
        public LogicProcessor(RulesGraph graph, ProcessorOptions options) : base(graph, options) {}
    }
}