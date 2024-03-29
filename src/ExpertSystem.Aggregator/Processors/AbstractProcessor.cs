using System;

namespace ExpertSystem.Aggregator.Processors
{
    public struct ProcessorOptions
    {
        public bool Debug;
    }

    public abstract class AbstractProcessor
    {
        protected readonly bool Debug;

        protected AbstractProcessor(ProcessorOptions options)
        {
            Debug = options.Debug;
        }

        protected void DebugWrite(string message)
        {
            if (Debug)
                Console.WriteLine(message);
        }
    }
}