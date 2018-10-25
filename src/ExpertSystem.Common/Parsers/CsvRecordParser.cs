using System.IO;
using System.Collections.Generic;
using ExpertSystem.Server.DAL.Serializers;

namespace ExpertSystem.Common.Parsers
{
    /// <summary>CSV парсер</summary>
    /// <typeparam name="T">Тип записи</typeparam>
    public class CsvRecordParser<T>
    {
        public IList<string> CsvHead { get; private set; }

        private readonly ICsvRecordSerializer<T> _serializer;

        public CsvRecordParser(ICsvRecordSerializer<T> serializer)
        {
            _serializer = serializer;
        }

        public List<T> ParseRecords(StreamReader reader)
        {
            var records = new List<T>();
            var data = CsvParser.ParseHeadAndTail(reader, _serializer.Delimiter, '"');

            CsvHead = data.Item1;
            foreach (var line in data.Item2)
                records.Add(_serializer.Deserialize(line));
            return records;
        }
    }
}