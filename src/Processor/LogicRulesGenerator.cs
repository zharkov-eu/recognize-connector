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
                foreach (var domain in CustomSocket.DOMAINS)
                    currentSocketFacts.AddLast(new LogicFact(
                        domain,
                        customSocketType.GetField(domain).GetValue(socket).ToString(),
                        CustomSocket.DOMAINS.Last().Equals(domain) ? Operations.Implication : Operations.Conjunction)
                    );
                currentSocketFacts.AddLast(new LogicFact("SocketName", socket.SocketName, Operations.None));
                socketsFacts.Add(currentSocketFacts);
            }

            return socketsFacts;
        }
    }
}