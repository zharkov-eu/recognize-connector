using System.Collections.Generic;
using System.IO;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Common.Parsers
{
    public static class SocketParser
    {
        public static IList<string> CsvHead;

        public static List<CustomSocket> ParseSockets(StreamReader reader)
        {
            var sockets = new List<CustomSocket>();
            var data = CsvParser.ParseHeadAndTail(reader, CustomSocketExtension.Delimiter, '"');
            CsvHead = data.Item1;
            var lines = data.Item2;

            foreach (var line in lines)
                sockets.Add(CustomSocketExtension.Deserialize(line));
            return sockets;
        }
    }
}