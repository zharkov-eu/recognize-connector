using System;
using System.Linq;
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

        public bool ForwardProcessing(FactSet factSet, string socketName)
        {
            Queue<GraphNode> queue = new Queue<GraphNode>();
            queue.Enqueue(graph.Root);
            while (queue.Count != 0)
            {
                var currentNode = queue.Dequeue();
                foreach (var node in currentNode.ChildNodes)
                {
                    if (node.FactSet.Facts.Except(factSet.Facts).Count() == 0)
                        queue.Enqueue(node);
                }
            }

            return false;
        }

        public FactSet BackProcessing(string socketName)
        {
            GraphNode targetSocket = null;

            // Осуществляем поиск в ширину
            Console.WriteLine("Осуществляем поиск в ширину");
            Queue<GraphNode> queue = new Queue<GraphNode>();
            queue.Enqueue(graph.Root);
            while (queue.Count != 0) {
                var node = queue.Dequeue();
                if (node != graph.Root) Console.WriteLine(node.ToString());

                if (node.SocketName == socketName)
                {
                    targetSocket = node;
                    break;
                }
                node.ChildNodes.ForEach(it => queue.Enqueue(it));
            }

            if (targetSocket == null) throw new System.Exception("Socket '{" + socketName + "}' not found");

            // Разворачиваем лист правил
            Console.WriteLine("Разворачиваем лист правил");
            FactSet facts = new FactSet();
            GraphNode currentNode = targetSocket;
            while (currentNode.ParentNode != null)
            {
                Console.WriteLine(currentNode.ToString());

                facts.Add(currentNode.FactSet.ToArray());
                currentNode = currentNode.ParentNode;
            }

            return facts;
        }
    }
}