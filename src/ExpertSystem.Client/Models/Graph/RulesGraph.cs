using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Client.Models.Graph
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