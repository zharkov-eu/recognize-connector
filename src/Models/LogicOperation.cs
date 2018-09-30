using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class LogicOperation
    {
        public enum Operations {
            None,
            Implication,
            Conjunction,
            Disjunction
        }

        public static Dictionary<Operations, int> Priority = new Dictionary<Operations, int>()
        {
            { Operations.None, 0 },
            { Operations.Implication, 3},
            { Operations.Conjunction, 2},
            { Operations.Disjunction, 1},
        };
    }
}