using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Models.Graph;

namespace ExpertSystem.ProductionProccessor
{
    public class Processor {
        public bool ForwardProcessing(HashSet<string> facts, string socketName) {
            return false;
        }

        public HashSet<string> BackProcessing(string socketName) {
            return new HashSet<string>();
        }
    }
}