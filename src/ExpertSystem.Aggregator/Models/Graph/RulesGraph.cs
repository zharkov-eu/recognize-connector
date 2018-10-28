using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Aggregator.Models.Graph
{
    public class RulesGraph
    {
        public GraphNode Root { get; }
        private readonly IReadOnlyCollection<SocketDomain> _domains;

        public RulesGraph(IReadOnlyCollection<SocketDomain> domains)
        {
            Root = new GraphNode(new Fact(
                SocketDomain.Empty,
                SocketDefaultValue[typeof(string)]
            ));
            _domains = domains;
        }

        public RulesGraph Compress()
        {
            Compress(Root);
            return this;
        }

        public void AddSocket(CustomSocket socket)
        {
            AddSocketRules(socket);
            Compress(Root);
        }

        public void AddSocketRules(CustomSocket socket)
        {
            // Начинаем с корня
            var currentNode = Root;

            // Итерируемся по списку доменов
            foreach (var domain in _domains)
            {
                GraphNode node = null;

                // Конструируем факт
                var facts = new FactSet(new Fact(
                    domain, CustomSocketExtension.SocketType.GetProperty(domain.ToString()).GetValue(socket)
                ));

                // Проверяем текущий список фактов, возможно, там уже есть этот факт
                foreach (var childNode in currentNode.ChildNodes)
                    if (facts.Equals(childNode.FactSet))
                        node = childNode;

                // В списке фактов нет этого факта, добавляем
                if (node == null)
                    node = currentNode.AddChild(new GraphNode(facts));

                // Если это последний домен - записываем название разъема
                if (domain == _domains.Last())
                    node.SocketName = socket.SocketName;

                currentNode = node;
            }
        }

        private static void Compress(GraphNode currentNode)
        {
            if (currentNode.ChildNodes.Count == 1)
            {
                var replaceNode = currentNode.ChildNodes.ElementAt(0);
                currentNode.FactSet.Add(replaceNode.FactSet.ToArray());
                if (replaceNode.SocketName != null)
                    currentNode.SocketName = replaceNode.SocketName;
                currentNode.ChildNodes = replaceNode.ChildNodes;
                if (currentNode.ChildNodes.Count == 1)
                    Compress(currentNode);
            }

            foreach (var node in currentNode.ChildNodes)
                Compress(node);
        }
    }
}