using System;
using ExpertSystem.Models.Graph;

namespace ExpertSystem.Processor
{
    public struct ProcessorOptions
    {
        public bool Debug;
    }

    public abstract class AbstractProcessor
    {
        protected readonly RulesGraph _graph;
        protected readonly bool _debug;

        protected AbstractProcessor(RulesGraph graph, ProcessorOptions options)
        {
            _graph = graph;
            _debug = options.Debug;
        }

        protected void debug(string message)
        {
            if (_debug) Console.WriteLine(message);
        }
    }
}