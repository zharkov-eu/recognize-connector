using System.Collections.Generic;
using ExpertSystem.Models;

namespace ExpertSystem.Processor
{
    public class LogicProcessor : AbstractProcessor
    {
        private List<LinkedList<LogicFact>> _facts;

        public LogicProcessor(List<LinkedList<LogicFact>> facts, ProcessorOptions options) : base(options)
        {
            _facts = facts;
        }
    }
}