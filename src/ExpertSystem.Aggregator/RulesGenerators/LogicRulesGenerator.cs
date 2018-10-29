using System.Collections.Generic;
using ExpertSystem.Aggregator.Models.CommonLogic;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Aggregator.RulesGenerators
{
    public class LogicRulesGenerator
    {
        public LogicRules GenerateRules(IEnumerable<CustomSocket> sockets)
        {
            var logicRules = new LogicRules();
            foreach (var socket in sockets)
                logicRules.AddSocket(socket);

            return logicRules;
        }
    }
}