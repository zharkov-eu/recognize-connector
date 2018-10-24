using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Models.Graph
{
    public class RulesGraph
    {
        public RulesGraph()
        {
            Root = new GraphNode(new Fact(
                CustomSocketDomain.SocketDomain.Empty,
                CustomSocketDomain.SocketDefaultValue[typeof(string)]
            ));
        }

        public GraphNode Root { get; }
    }
}