using ExpertSystem.Server.DAL.Entities;

namespace ExpertSystem.Server.DAL.Serializers
{
    public class WalEntrySerializer<T> : IRecordSerializer<WalEntry<T>>
    {
        private const string Delimiter = ":::";

        private readonly IRecordSerializer<T> _serializer;

        public WalEntrySerializer(IRecordSerializer<T> serializer)
        {
            _serializer = serializer;
        }

        public string Serialize(WalEntry<T> record)
        {
            return record.Action + Delimiter + record.HashCode + Delimiter
                   + (record.Record != null ? _serializer.Serialize(record.Record) : "");
        }

        public WalEntry<T> Deserialize(string line)
        {
            var parts = line.Split(Delimiter);
            var action = (CsvDbAction) int.Parse(parts[0]);
            var hashCode = int.Parse(parts[1]);
            var socket = _serializer.Deserialize(parts[2]);
            return new WalEntry<T>(action, hashCode, socket);
        }
    }
}