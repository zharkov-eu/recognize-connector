using System.Collections.Generic;

namespace ExpertSystem.Server.DAL.Serializers
{
    public interface ICsvRecordSerializer<T> : IRecordSerializer<T>
    {
        char Delimiter { get; }

        T Deserialize(IList<string> line);
    }
}