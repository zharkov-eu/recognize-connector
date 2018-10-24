using System.Collections.Generic;
using System.IO;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Common.Parsers
{
    /// <summary>CSV парсер</summary>
    /// <typeparam name="TR">Тип записи</typeparam>
    /// <typeparam name="TE">Тип расширения</typeparam>
    public class CustomParser<TR, TE>
        where TE : RecordExtension<TR>
    {
        public static IList<string> CsvHead;

        public static List<TR> ParseRecords(StreamReader reader)
        {
            var records = new List<TR>();
            var data = CsvParser.ParseHeadAndTail(reader, TE.Delimiter, '"');
            CsvHead = data.Item1;
            var lines = data.Item2;

            foreach (var line in lines)
                records.Add(TE.Deserialize(line));
            return records;
        }
    }
}