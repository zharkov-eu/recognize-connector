using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models.Graph
{
    public class RulesGraph
    {
        public RulesGraph()
        {
            Root = new GraphNode(new Fact(SocketDomain.Empty, SocketDefaultValue[typeof(string)]));
        }

        public GraphNode Root { get; set; }
    }
}