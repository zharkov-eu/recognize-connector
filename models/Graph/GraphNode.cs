﻿using System.Collections.Generic;

namespace ExpertSystem.Models.Graph
{
    public class GraphNode
    {
        public int LevelNum { get; set; }

        public List<string> FactsList { set; get; }

        public GraphNode ParentNode { get; set; }

        public GraphNode ChildNode { get; set; }

        public string SocketName { get; set; }
    }
}
