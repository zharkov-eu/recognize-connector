using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Common.Serializers
{
    public class SocketGroupSerializer : ICsvRecordSerializer<SocketGroup>
    {
        public char Delimiter { get; }

        public SocketGroupSerializer()
        {
            Delimiter = SocketGroupExtension.Delimiter;
        }

        public string Serialize(SocketGroup group)
        {
            return SocketGroupExtension.Serialize(group);
        }

        public SocketGroup Deserialize(string line)
        {
            return SocketGroupExtension.Deserialize(line);
        }

        public SocketGroup Deserialize(IList<string> parts)
        {
            return SocketGroupExtension.Deserialize(parts);
        }
    }
}