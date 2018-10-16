using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Models.CommonLogic;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Common.RulesGenerators
{
    public class LogicRulesGenerator
    {
        public List<LinkedList<LogicFact>> GenerateRules(List<CustomSocket> sockets)
        {
            var socketsFacts = new List<LinkedList<LogicFact>>();

            foreach (var socket in sockets)
            {
                var currentSocketFacts = new LinkedList<LogicFact>();
                foreach (var domain in GetSocketDomains())
                {
                    //var type = SocketDomainType[domain];
                    var operation = domain.Equals(SocketDomain.SocketName)
                        ? LogicOperation.Operation.Implication
                        : LogicOperation.Operation.Conjunction;
                    var fact = new LogicFact(domain, CustomSocket.Type.GetField(domain.ToString()).GetValue(socket),
                        operation);
                    if (!fact.IsDefaultValue())
                        currentSocketFacts.AddLast(fact);
                }

                currentSocketFacts.AddLast(new LogicFact(SocketDomain.SocketName, socket.SocketName,
                    LogicOperation.Operation.None));
                socketsFacts.Add(currentSocketFacts);
            }

            return socketsFacts;
        }
    }
}