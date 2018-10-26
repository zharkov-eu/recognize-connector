using System;

namespace ExpertSystem.Aggregator.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message)
        {}
    }
}