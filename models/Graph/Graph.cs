using System.Collections.Generic;
namespace ExpertSystem.Models.Graph
{
    public class RulesGraph
    {
        public static readonly string ROOT_DOMAIN = ".";
        public List<GraphNode> NodesList { get; set; }

        public RulesGraph() {
            NodesList = new List<GraphNode>();
            GraphNode root = new GraphNode(new Fact(ROOT_DOMAIN, null));
            NodesList.Add(root);
        }
    }
}
