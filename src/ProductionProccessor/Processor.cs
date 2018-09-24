using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Models;
using ExpertSystem.Models.Graph;

namespace ExpertSystem.ProductionProccessor
{
    public class Processor
    {
        private readonly RulesGraph _graph;

        public Processor(RulesGraph graph)
        {
            _graph = graph;
        }

        public bool ForwardProcessing(FactSet factSet, string socketName)
        {
            var queue = new Queue<GraphNode>();
            queue.Enqueue(_graph.Root);
            while (queue.Count != 0)
            {
                var currentNode = queue.Dequeue();
                foreach (var node in currentNode.ChildNodes)
                    if (!node.FactSet.Facts.Except(factSet.Facts).Any())
                        queue.Enqueue(node);
            }

            return false;
        }

        public FactSet BackProcessing(string socketName)
        {
            GraphNode targetSocket = null;

            // Осуществляем поиск в ширину
            var queue = new Queue<GraphNode>();
            queue.Enqueue(_graph.Root);
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                if (node.SocketName == socketName)
                {
                    targetSocket = node;
                    break;
                }

                node.ChildNodes.ForEach(it => queue.Enqueue(it));
            }

            if (targetSocket == null) throw new Exception("Socket '{" + socketName + "}' not found");

            // Разворачиваем лист правил
            var facts = new FactSet();
            var currentNode = targetSocket;
            while (currentNode.ParentNode != null)
            {
                facts.Add(currentNode.FactSet.ToArray());
                currentNode = currentNode.ParentNode;
            }

            return facts;
        }
    }
}