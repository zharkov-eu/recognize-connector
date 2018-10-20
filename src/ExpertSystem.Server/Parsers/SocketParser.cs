using System.IO;
using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Entities;

namespace ExpertSystem.Server.Parsers
{
    public static class SocketParser
    {
        public static IList<string> CsvHead;

        public static List<CustomSocket> ParseSockets(StreamReader reader)
        {
            var sockets = new List<CustomSocket>();
            var data = CsvParser.ParseHeadAndTail(reader, StorageCustomSocket.Delimiter, '"');
            CsvHead = data.Item1;
            var lines = data.Item2;

            foreach (var line in lines)
                sockets.Add(StorageCustomSocket.Deserialize(line));
            return sockets;
        }
    }
}