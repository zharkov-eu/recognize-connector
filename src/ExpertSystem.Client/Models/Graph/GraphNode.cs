using System.Collections.Generic;

namespace ExpertSystem.Client.Models.Graph
{
    public class GraphNode
    {
        public GraphNode(params Fact[] facts)
        {
            FactSet = new FactSet(facts);
            ChildNodes = new List<GraphNode>();
        }

        public GraphNode(FactSet facts)
        {
            FactSet = facts;
            ChildNodes = new List<GraphNode>();
        }

        public FactSet FactSet { set; get; }
        public GraphNode ParentNode { get; set; }
        public List<GraphNode> ChildNodes { get; set; }
        public string SocketName { get; set; }

        public GraphNode AddChild(GraphNode node)
        {
            ChildNodes.Add(node);
            node.ParentNode = this;
            return node;
        }

        public override string ToString()
        {
            var rules = new List<string>();
            if (ParentNode != null) rules.Add(ParentNode.GetHashCode().ToString());
            foreach (var Fact in FactSet.Facts)
                if (!Fact.IsDefaultValue())
                    rules.Add($"({Fact.Domain}: {Fact.Value})");
            var result = string.IsNullOrEmpty(SocketName) ? GetHashCode().ToString() : SocketName;
            return $"{string.Join(" & ", rules.ToArray())} -> {result}";
        }
    }
}