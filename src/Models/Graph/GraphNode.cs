using System.Collections.Generic;

namespace ExpertSystem.Models.Graph
{
    public class GraphNode
    {
        public FactSet Facts { set; get; }
        public GraphNode ParentNode { get; set; }
        public List<GraphNode> ChildNodes { get; set; }
        public string SocketName { get; set; }

        public GraphNode(params Fact[] facts) {
            Facts = new FactSet(facts);
            ChildNodes = new List<GraphNode>();
        }

        public GraphNode(FactSet facts) {
            Facts = facts;
            ChildNodes = new List<GraphNode>();
        }

        public GraphNode AddChild(GraphNode node) {
            ChildNodes.Add(node);
            node.ParentNode = this;
            return node;
        }
    }
}
