using System;
using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Models.Graph;

namespace ExpertSystem.Processor.ProductionProcessor
{
    public class ProductionProcessor : AbstractProcessor
    {
        private readonly RulesGraph _graph;

        public ProductionProcessor(RulesGraph graph, ProcessorOptions options)
            : base(options)
        {
            _graph = graph;
        }

        /// <summary> 
        /// Прямой продукционный вывод
        /// </summary>
        /// <param name="factSet">Множество входных фактов</param>
        /// <example>
        /// Поиск разъемов для множества входных фактов
        /// <code>
        /// var numberOfPositions = new Fact("NumberOfPositions", "60");
        /// var numberOfContacts = new Fact("NumberOfContacts", "120");
        /// var gender = new Fact("Gender", "Female");
        /// FactSet factSet = new FactSet(numberOfPositions, numberOfContacts, gender);
        /// var socketList = processor.ForwardProcessing(factSet);
        /// </code>
        /// </example>
        public List<string> ForwardProcessing(FactSet factSet)
        {
            var socketList = new List<string>();
            var queue = new Queue<GraphNode>();
            queue.Enqueue(_graph.Root);
            while (queue.Count != 0)
            {
                var currentNode = queue.Dequeue();
                debug(currentNode.ToString());

                if (currentNode.SocketName != null)
                    socketList.Add(currentNode.SocketName);
                foreach (var node in currentNode.ChildNodes)
                    if (CompareDomains(factSet, node.FactSet))
                        queue.Enqueue(node);
            }

            return socketList;
        }

        /// <summary>
        /// Обратный продукционный вывод
        /// </summary>
        /// <param name="socketName">Наименование разъема</param>
        /// <example>
        /// Поиск фактов для разъема 5145167-4
        /// <code>
        /// FactSet set = processor.BackProcessing("5145167-4")
        /// </code>
        /// </example>
        public FactSet BackProcessing(string socketName)
        {
            GraphNode targetSocket = null;

            // Осуществляем поиск в ширину
            debug("Осуществляем поиск в ширину");
            var queue = new Queue<GraphNode>();
            queue.Enqueue(_graph.Root);
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                if (node != _graph.Root) debug(node.ToString());

                if (node.SocketName == socketName)
                {
                    targetSocket = node;
                    break;
                }
                node.ChildNodes.ForEach(it => queue.Enqueue(it));
            }

            if (targetSocket == null)
                throw new Exception("Разъем '{" + socketName + "}' не найден");

            // Разворачиваем лист правил
            debug("Разворачиваем лист правил");
            var facts = new FactSet();
            var currentNode = targetSocket;
            while (currentNode.ParentNode != null)
            {
                debug(currentNode.ToString());
                facts.Add(currentNode.FactSet.ToArray());
                currentNode = currentNode.ParentNode;
            }

            return facts;
        }

        private static bool CompareDomains(FactSet current, FactSet expected)
        {
            var compared = true;
            var facts = new Dictionary<string, object>();
            foreach (var fact in expected.Facts)
                facts.Add(fact.Domain, fact.Value);
            foreach (var fact in current.Facts)
                if (facts.ContainsKey(fact.Domain) && !facts[fact.Domain].Equals(fact.Value))
                    compared = false;
            return compared;
        }
    }
}