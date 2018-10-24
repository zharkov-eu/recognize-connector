using System.Collections.Generic;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Models.Graph
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
            foreach (var fact in FactSet.Facts)
                if (!fact.IsDefaultValue())
                    rules.Add($"({fact.Domain}: {fact.Value})");
            var result = string.IsNullOrEmpty(SocketName) ? GetHashCode().ToString() : SocketName;
            return $"{string.Join(" & ", rules.ToArray())} -> {result}";
        }
    }
}