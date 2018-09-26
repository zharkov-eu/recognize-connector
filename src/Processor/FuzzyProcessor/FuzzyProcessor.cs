using ExpertSystem.Models.Graph;

namespace ExpertSystem.Processor
{
    public class FuzzyProcessor : AbstractProcessor
    {
        public FuzzyProcessor(RulesGraph graph, ProcessorOptions options) : base(graph, options) {}
    }
}