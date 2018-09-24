using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Models.Graph;

namespace ExpertSystem.ProductionProccessor
{
    public class Processor {
        private RulesGraph graph;

        public Processor(RulesGraph graph)
        {
            this.graph = graph;
        }

        public bool ForwardProcessing(HashSet<string> facts, string socketName)
        {
            return false;
        }

        public FactSet BackProcessing(string socketName)
        {
            GraphNode targetSocket = null;

            // Осуществляем поиск в ширину
            Queue<GraphNode> queue = new Queue<GraphNode>();
            queue.Enqueue(graph.Root);
            while (queue.Count != 0) {
                var node = queue.Dequeue();
                if (node.SocketName == socketName)
                {
                    targetSocket = node;
                    break;
                }
                node.ChildNodes.ForEach(it => queue.Enqueue(it));
            }

            if (targetSocket == null) throw new System.Exception("Socket '{" + socketName + "}' not found");

            // Разворачиваем лист правил
            FactSet facts = new FactSet();
            GraphNode currentNode = targetSocket;
            while (currentNode.ParentNode != null)
            {
                facts.Add(currentNode.Facts.ToArray());
                currentNode = currentNode.ParentNode;
            }

            return facts;
        }
    }
}