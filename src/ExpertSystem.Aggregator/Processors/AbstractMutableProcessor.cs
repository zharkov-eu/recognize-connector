using ExpertSystem.Common.Generated;

namespace ExpertSystem.Aggregator.Processors
{
    public abstract class AbstractMutableProcessor : AbstractProcessor
    {
        protected AbstractMutableProcessor(ProcessorOptions options) : base(options) {}

        public abstract void AddSocket(CustomSocket socket);

        public abstract void RemoveSocket(CustomSocket socket);
    }
}