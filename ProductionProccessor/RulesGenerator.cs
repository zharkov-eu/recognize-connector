using System.Collections.Generic;
using System.Linq;

namespace ExpertSystem.ProductionProccessor
{
    public class RulesGenerator
    {
        public void GenerateRules(Dictionary<string, List<string>> fieldsWithValues)
        {
            //Сортировка полей по числу принимаемых ими значений
            fieldsWithValues = fieldsWithValues.OrderBy(p => p.Value.Count).ToDictionary(x => x.Key, x => x.Value);



        }
    }
}
