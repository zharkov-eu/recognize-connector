using System.Collections.Generic;
using ExpertSystem.Common.Generated;

namespace ExpertSystem.Server.DAL.Serializers
{
    public class SocketGroupSerializer : ICsvRecordSerializer<SocketGroup>
    {
        public char Delimiter { get; }

        public string Serialize(SocketGroup record)
        {
            throw new System.NotImplementedException();
        }

        public SocketGroup Deserialize(string line)
        {
            throw new System.NotImplementedException();
        }

        public SocketGroup Deserialize(IList<string> parts)
        {
            throw new System.NotImplementedException();
        }
    }
}