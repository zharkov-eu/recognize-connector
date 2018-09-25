using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Models.Graph;

namespace ExpertSystem.ProductionProccessor
{
    public struct ProcessorOptions
    {
        public bool Debug;
    }
    public class Processor
    {
        private readonly RulesGraph _graph;
        private readonly bool _debug;

        public Processor(RulesGraph graph, ProcessorOptions options)
        {
            _graph = graph;
            _debug = options.Debug;
        }

        public List<string> ForwardProcessing(FactSet factSet)
        {
            List<string> socketList = new List<string>();
            var queue = new Queue<GraphNode>();
            queue.Enqueue(_graph.Root);
            while (queue.Count != 0)
            {
                var currentNode = queue.Dequeue();
                debug(currentNode.ToString());

                if (currentNode.SocketName != null)
                    socketList.Add(currentNode.SocketName);
                foreach (var node in currentNode.ChildNodes)
                    if (compareDomains(factSet, node.FactSet))
                        queue.Enqueue(node);
            }

            return socketList;
        }

        public FactSet BackProcessing(string socketName)
        {
            GraphNode targetSocket = null;

            // Осуществляем поиск в ширину
            debug("Осуществляем поиск в ширину");
            Queue<GraphNode> queue = new Queue<GraphNode>();
            queue.Enqueue(_graph.Root);
            while (queue.Count != 0) {
                var node = queue.Dequeue();
                if (node != _graph.Root) debug(node.ToString());

                if (node.SocketName == socketName)
                {
                    targetSocket = node;
                    break;
                }
                node.ChildNodes.ForEach(it => queue.Enqueue(it));
            }

            if (targetSocket == null) throw new Exception("Socket '{" + socketName + "}' not found");

            // Разворачиваем лист правил
            debug("Разворачиваем лист правил");
            FactSet facts = new FactSet();
            GraphNode currentNode = targetSocket;
            while (currentNode.ParentNode != null)
            {
                debug(currentNode.ToString());
                facts.Add(currentNode.FactSet.ToArray());
                currentNode = currentNode.ParentNode;
            }

            return facts;
        }

        private bool compareDomains(FactSet current, FactSet expected)
        {
            bool compared = true;
            Dictionary<string, string> facts = new Dictionary<string, string>();
            foreach (var fact in expected.Facts)
                facts.Add(fact.Domain, fact.Value);
            foreach (var fact in current.Facts)
                if (facts.ContainsKey(fact.Domain) && facts[fact.Domain] != fact.Value)
                    compared = false;
            return compared;
        }

        private void debug(string message)
        {
            if (_debug) Console.WriteLine(message);
        }
    }
}