using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Server.DAL.Serializers
{
    public class CustomSocketSerializer : ICsvRecordSerializer<CustomSocket>
    {
        public char Delimiter { get; }

        public CustomSocketSerializer()
        {
            Delimiter = CustomSocketExtension.Delimiter;
        }

        public string Serialize(CustomSocket socket)
        {
            return CustomSocketExtension.Serialize(socket);
        }

        public CustomSocket Deserialize(string line)
        {
            return CustomSocketExtension.Deserialize(line);
        }

        public CustomSocket Deserialize(IList<string> parts)
        {
            return CustomSocketExtension.Deserialize(parts);
        }
    }
}