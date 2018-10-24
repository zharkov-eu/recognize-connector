using System.Collections.Generic;
using System.IO;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Common.Parsers
{
    /// <summary>CSV парсер</summary>
    /// <typeparam name="T">Тип записи</typeparam>
    public class CustomParser<T>
    {
        public IList<string> CsvHead { get; private set; }

        private IRecordExtension<T> _extension;

        public CustomParser(IRecordExtension<T> extension)
        {
            _extension = extension;
        }

        public List<T> ParseRecords(StreamReader reader)
        {
            var records = new List<T>();
            var data = CsvParser.ParseHeadAndTail(reader, _extension.Delimiter, '"');
            CsvHead = data.Item1;
            var lines = data.Item2;

            foreach (var line in lines)
                records.Add(_extension.Deserialize(line));
            return records;
        }
    }
}