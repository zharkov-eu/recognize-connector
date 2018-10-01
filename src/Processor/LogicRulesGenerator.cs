using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.LogicOperation;

namespace ExpertSystem.Processor
{
    public class LogicRulesGenerator
    {
        public List<LinkedList<LogicFact>> GenerateRules(List<CustomSocket> sockets)
        {
            var socketsFacts = new List<LinkedList<LogicFact>>();
            var customSocketType = typeof(CustomSocket);

            foreach (var socket in sockets)
            {
                LinkedList<LogicFact> currentSocketFacts = new LinkedList<LogicFact>();
                foreach (var domain in CustomSocket.Domains.Keys)
                {
                    Type type = CustomSocket.Domains[domain];
                    Operation operation = CustomSocket.Domains.Last().Equals(domain) ? Operation.Implication : Operation.Conjunction;
                    LogicFact fact = new LogicFact(domain, customSocketType.GetField(domain).GetValue(socket), type, operation);
                    if (!fact.IsDefaultValue())
                        currentSocketFacts.AddLast(fact);
                }
                currentSocketFacts.AddLast(new LogicFact("SocketName", socket.SocketName, typeof(string), Operation.None));
                socketsFacts.Add(currentSocketFacts);
            }

            return socketsFacts;
        }
    }
}