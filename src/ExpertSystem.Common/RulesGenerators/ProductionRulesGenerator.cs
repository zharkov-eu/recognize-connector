using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Models;
using ExpertSystem.Models.Graph;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Common.RulesGenerators
{
    public class ProductionRulesGenerator
    {
        public RulesGraph GenerateRules(List<CustomSocket> sockets, Dictionary<SocketDomain, List<string>> domainValues)
        {
            var rulesGraph = new RulesGraph();

            //Сортировка полей по числу принимаемых ими значений
            domainValues = domainValues.OrderBy(p => p.Value.Count).ToDictionary(x => x.Key, x => x.Value);
            var domains = domainValues.Keys.ToArray();

            foreach (var socket in sockets)
            {
                // Начинаем с корня
                var currentNode = rulesGraph.Root;

                // Итерируемся по списку доменов
                foreach (var domain in domains)
                {
                    GraphNode node = null;

                    // Конструируем факт
                    //var type = SocketDomainType[domain];
                    var facts = new FactSet(
                        new Fact(domain, CustomSocket.Type.GetField(domain.ToString()).GetValue(socket))
                    );

                    // Проверяем текущий список фактов, возможно, там уже есть этот факт
                    foreach (var childNode in currentNode.ChildNodes)
                        if (facts.Equals(childNode.FactSet))
                            node = childNode;

                    // В списке фактов нет этого факта, добавляем
                    if (node == null)
                        node = currentNode.AddChild(new GraphNode(facts));

                    // Если это последний домен - записываем название разъема
                    if (domain == domains.Last())
                        node.SocketName = socket.SocketName;

                    currentNode = node;
                }
            }

            // Сжимаем граф
            Compress(rulesGraph.Root);
            return rulesGraph;
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