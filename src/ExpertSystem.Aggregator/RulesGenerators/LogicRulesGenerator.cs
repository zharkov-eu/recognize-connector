using System.Collections.Generic;
using ExpertSystem.Aggregator.Models.CommonLogic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.RulesGenerators
{
    public class LogicRulesGenerator
    {
        public List<LinkedList<LogicFact>> GenerateRules(List<CustomSocket> sockets)
        {
            var socketsFacts = new List<LinkedList<LogicFact>>();

            foreach (var socket in sockets)
            {
                var currentSocketFacts = new LinkedList<LogicFact>();
                foreach (var domain in CustomSocketDomain.GetSocketDomains())
                {
                    var operation = domain.Equals(CustomSocketDomain.SocketDomain.SocketName)
                        ? LogicOperation.Operation.Implication
                        : LogicOperation.Operation.Conjunction;
                    var fact = new LogicFact(domain,
                        CustomSocketExtension.SocketType.GetProperty(domain.ToString()).GetValue(socket),
                        operation);
                    if (!fact.IsDefaultValue())
                        currentSocketFacts.AddLast(fact);
                }

                currentSocketFacts.AddLast(new LogicFact(CustomSocketDomain.SocketDomain.SocketName, socket.SocketName,
                    LogicOperation.Operation.None));
                socketsFacts.Add(currentSocketFacts);
            }

            return socketsFacts;
        }
    }
}