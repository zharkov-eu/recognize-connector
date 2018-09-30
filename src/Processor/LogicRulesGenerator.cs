using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;

namespace ExpertSystem.Processor
{
    public class LogicRulesGenerator
    {
        public List<LinkedList<LogicFact>> GenerateRules(List<CustomSocket> sockets)
        {
            List<LinkedList<LogicFact>> socketsFacts = new List<LinkedList<LogicFact>>();
            var customSocketType = typeof(CustomSocket);

            foreach (var socket in sockets)
            {
                LinkedList<LogicFact> currentSocketFacts = new LinkedList<LogicFact>();
                foreach (var domain in CustomSocket.DOMAINS)
                    currentSocketFacts.AddLast(new LogicFact(
                        domain,
                        customSocketType.GetField(domain).GetValue(socket).ToString(), 
                        CustomSocket.DOMAINS.Last().Equals(domain) ? LogicOperation.None : LogicOperation.Conjunction)
                    );
                socketsFacts.Add(currentSocketFacts);
            }

            return socketsFacts;
        }
    }
}