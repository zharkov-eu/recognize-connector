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
        protected readonly bool _debug;

        protected AbstractProcessor(ProcessorOptions options)
        {
            _debug = options.Debug;
        }

        protected void debug(string message)
        {
            if (_debug) Console.WriteLine(message);
        }
    }
}