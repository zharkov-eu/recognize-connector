﻿using System.Collections.Generic;

namespace ExpertSystem.Models.Graph
{
    public class GraphNode
    {
        public FactSet FactSet { set; get; }
        public GraphNode ParentNode { get; set; }
        public List<GraphNode> ChildNodes { get; set; }
        public string SocketName { get; set; }

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

        public GraphNode AddChild(GraphNode node)
        {
            ChildNodes.Add(node);
            node.ParentNode = this;
            return node;
        }

        public override string ToString() {
            List<string> rules = new List<string>();
            if (ParentNode != null) rules.Add(ParentNode.GetHashCode().ToString());
            foreach (var Fact in FactSet.Facts)
                if (Fact.Value != "") rules.Add($"({Fact.Domain}: {Fact.Value})");
            string result = string.IsNullOrEmpty(SocketName) ? this.GetHashCode().ToString() : SocketName;
            return $"{string.Join(" & ", rules.ToArray())} -> {result}";
        }
    }
}