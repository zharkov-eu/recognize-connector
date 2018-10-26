using System.Collections.Generic;

namespace ExpertSystem.Common.Serializers
{
    public interface ICsvRecordSerializer<T> : IRecordSerializer<T>
    {
        char Delimiter { get; }

        T Deserialize(IList<string> line);
    }
}