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
            //Сортировка полей по числу принимаемых ими значений
            fieldsValues = fieldsValues.OrderBy(p => p.Value.Count).ToDictionary(x => x.Key, x => x.Value);
            var valuesArray = fieldsValues.Keys.ToArray();

            var customSocketType = typeof(CustomSocket);
            var rulesGraph = new RulesGraph();

            //Пробегаемся по всем свойствам разъема, тем самым спускаясь по дереву правил
            for (var i = 0; i < valuesArray.Length; i++)
            {
            }

            return rulesGraph;
        }
    }
}
