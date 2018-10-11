using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using static ExpertSystem.Models.LogicOperation;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor
{
    public class LogicRulesGenerator
    {
        public List<LinkedList<LogicFact>> GenerateRules(List<CustomSocket> sockets)
        {
            var socketsFacts = new List<LinkedList<LogicFact>>();

            foreach (var socket in sockets)
            {
                LinkedList<LogicFact> currentSocketFacts = new LinkedList<LogicFact>();
                foreach (SocketDomain domain in GetSocketDomains())
                {
                    Type type = SocketDomainType[domain];
                    Operation operation = domain.Equals(SocketDomain.SocketName) ? Operation.Implication : Operation.Conjunction;
                    LogicFact fact = new LogicFact(domain, CustomSocket.Type.GetField(domain.ToString()).GetValue(socket), operation);
                    if (!fact.IsDefaultValue())
                        currentSocketFacts.AddLast(fact);
                }
                currentSocketFacts.AddLast(new LogicFact(SocketDomain.SocketName, socket.SocketName, Operation.None));
                socketsFacts.Add(currentSocketFacts);
            }

            return socketsFacts;
        }
    }
}