using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Models.Graph
{
    public class RulesGraph
    {
        public GraphNode Root { get; set; }

        public RulesGraph()
        {
            Root = new GraphNode(new Fact(
                SocketDomain.Empty, SocketDefaultValue[typeof(string)], typeof(string)
            ));
        }
    }
}