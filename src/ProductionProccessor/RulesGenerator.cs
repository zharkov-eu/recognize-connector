using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Models;
using ExpertSystem.Models.Graph;

namespace ExpertSystem.ProductionProccessor
{
    public class RulesGenerator
    {
        public RulesGraph GenerateRules(List<CustomSocket> sockets, Dictionary<string, List<string>> fieldsValues)
        {
            var rulesGraph = new RulesGraph();

            //Сортировка полей по числу принимаемых ими значений
            fieldsValues = fieldsValues.OrderBy(p => p.Value.Count).ToDictionary(x => x.Key, x => x.Value);
            var domains = fieldsValues.Keys.ToArray();

            var customSocketType = typeof(CustomSocket);

            foreach (var socket in sockets)
            {
                // Начинаем с корня
                GraphNode currentNode = rulesGraph.Root;

                // Итерируемся по списку доменов
                foreach (var domain in domains)
                {
                    GraphNode node = null;

                    // Конструируем факт
                    FactSet facts = new FactSet(
                        new Fact(domain, (string)customSocketType.GetField(domain).GetValue(socket))
                    );

                    // Проверяем текущий список фактов, возможно, там уже есть этот факт
                    foreach (var childNode in currentNode.ChildNodes)
                        if (facts.Equals(childNode.Facts)) node = childNode;

                    // В списке фактов нет этого факта, добавляем
                    if (node == null)
                        node = currentNode.AddChild(new GraphNode(facts));

                    currentNode = node;
                }
            }

            // Сжимаем граф
            compress(rulesGraph.Root);
            
            return rulesGraph;
        }

        private void compress(GraphNode currentNode) {
            if (currentNode.ChildNodes.Count == 1)
            {
                currentNode.Facts.Add(currentNode.ChildNodes.ElementAt(0).Facts.ToArray());
                currentNode.ChildNodes.RemoveAt(0);
            }
            foreach (var node in currentNode.ChildNodes)
                compress(node);
        }
    }
}
