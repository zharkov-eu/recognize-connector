using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Models;
using ExpertSystem.Models.Graph;

namespace ExpertSystem.ProductionProccessor
{
    public class RulesGenerator
    {
        public void GenerateRules(Dictionary<string, List<string>> fieldsWithValues, List<CustomSocket> sockets)
        {
            //Сортировка полей по числу принимаемых ими значений
            fieldsWithValues = fieldsWithValues.OrderBy(p => p.Value.Count).ToDictionary(x => x.Key, x => x.Value);
            var valuesArray = fieldsWithValues.Keys.ToArray();

            var customSocketType = typeof(CustomSocket);
            var rulesGraph = new RulesGraph();

            //Пробегаемся по всем свойствам разъема, тем самым спускаясь по дереву правил
            for (var i = 0; i < valuesArray.Length; i++)
            {
                foreach (var fieldValue in fieldsWithValues[valuesArray[i]])
                {
                    var newNode = new GraphNode();
                    //Если i==0 - то спускаемся от корня древа
                    if (i == 0)
                        newNode.FactsList = new List<string> {fieldValue};
                    else
                    {
                        
                    }
                    //rulesGraph.NodesList.Add(
                    //});

                   

                    //Если нет разъемов с полем следующего уровня дерева - записываем узел как лист
                    var nextLevelField = customSocketType.GetField(valuesArray[i + 1]);
                    if (!sockets.Any(p => string.IsNullOrEmpty((string)nextLevelField.GetValue(p))))
                    {
                        //ищем разъем со свойствами из списка фактов текущего узла
                        //sockets.Where(p=>p.)
                        newNode.SocketName = "i dont know";
                    }
                }
            }

        }
    }
}
