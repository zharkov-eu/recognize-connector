using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Server.Parsers
{
    public class SocketFieldsProcessor
    {
        public List<CustomSocket> GetSockets(Stream stream)
        {
            var entries = new List<CustomSocket>();
            using (var reader = new StreamReader(stream))
            {
                var data = CsvParser.ParseHeadAndTail(reader, ';', '"');
                var lines = data.Item2;

                foreach (var line in lines)
                    if (line != null && line.Any())
                        entries.Add(new CustomSocket
                        {
                            SocketName = line[0],
                            Gender = line[1],
                            ContactMaterial = line[2],
                            ContactPlating = line[3],
                            Color = line[4],
                            HousingColor = line[5],
                            HousingMaterial = line[6],
                            MountingStyle = line[7],
                            NumberOfContacts = string.IsNullOrEmpty(line[8]) ? -1 : int.Parse(line[8]),
                            NumberOfPositions = string.IsNullOrEmpty(line[9]) ? -1 : int.Parse(line[9]),
                            NumberOfRows = string.IsNullOrEmpty(line[10]) ? -1 : int.Parse(line[10]),
                            Orientation = line[11],
                            PinPitch = string.IsNullOrEmpty(line[12])
                                ? -1.0f
                                : float.Parse(line[12], CultureInfo.InvariantCulture),
                            Material = line[13],
                            SizeDiameter =
                                string.IsNullOrEmpty(line[14])
                                    ? -1.0f
                                    : float.Parse(line[14], CultureInfo.InvariantCulture),
                            SizeLength =
                                string.IsNullOrEmpty(line[15])
                                    ? -1.0f
                                    : float.Parse(line[15], CultureInfo.InvariantCulture),
                            SizeHeight =
                                string.IsNullOrEmpty(line[16])
                                    ? -1.0f
                                    : float.Parse(line[16], CultureInfo.InvariantCulture),
                            SizeWidth = string.IsNullOrEmpty(line[17])
                                ? -1.0f
                                : float.Parse(line[17], CultureInfo.InvariantCulture)
                        });
            }

            return entries;
        }
    }
}
