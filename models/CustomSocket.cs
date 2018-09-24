using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpertSystem.Parse;

namespace ExpertSystem.Models
{
    public class CustomSocket {
        public string SocketName {get; set; }
        public string Gender { get; set; }
        public string ContactMaterial { get; set; }
        public string ContactPlating { get; set; }
        public string Color { get; set; }
        public string HousingColor { get; set; }
        public string HousingMaterial { get; set; }
        public string MountingStyle { get; set; }
        public string NumberOfContacts { get; set; }
        public string NumberOfPositions { get; set; }
        public string NumberOfRows { get; set; }
        public string Orientation { get; set; }
        public string PinPitch { get; set; }
        public string Material { get; set; }
        public string SizeDiameter { get; set; }
        public string SizeLength { get; set; }
        public string SizeHeight { get; set; }
        public string SizeWidth { get; set; }         
    }

    public class SocketFieldsProcessor
    {
        private static readonly string[] ORDER_BY =
        {
            "Gender",
            "ContactMaterial",
            "ContactPlating",
            "Color",
            "HousingColor",
            "HousingMaterial",
            "MountingStyle",
            "NumberOfContacts",
            "NumberOfPositions",
            "NumberOfRows",
            "Orientation",
            "PinPitch",
            "Material",
            "SizeDiameter",
            "SizeLength",
            "SizeHeight",
            "SizeWidth"
        };

        public Dictionary<string, List<string>> GetFieldsWithPossibleValues()
        {
            var fileName = Directory.GetCurrentDirectory() + "\\1.csv";

            var entries = new List<CustomSocket>();
            using (var stream = File.OpenRead(fileName))
            using (var reader = new StreamReader(stream))
            {
                var data = CsvParser.ParseHeadAndTail(reader, ',', '"');
                var lines = data.Item2;
            
                foreach (var line in lines)
                {
                    if (line!=null && line.Any())
                    {
                        entries.Add(new CustomSocket
                        {
                            SocketName = line[0],
                            Color = line[1],
                            ContactMaterial = line[2],
                            ContactPlating = line[3],
                            Gender = line[4],
                            HousingColor = line[5],
                            HousingMaterial = line[6],
                            NumberOfContacts = line[7],
                            NumberOfPositions = line[8],
                            NumberOfRows = line[9],
                            Orientation = line[10],
                            PinPitch = line[11],
                            Material = line[12],
                            SizeDiameter = line[13],
                            SizeLength = line[14],
                            SizeHeight = line[15],
                            SizeWidth = line[16]
                        });
                    }                  
                }
            }

            var fieldsValues = new Dictionary<string, List<string>>();
            var customSocketType = typeof(CustomSocket);
            foreach (var property in ORDER_BY)
            {
                var field = customSocketType.GetField(property);

                var propertyValues = entries.GroupBy(p => (string)field.GetValue(p)).ToList();
                var currentPropValues = new List<string>();
                foreach (var value in propertyValues)
                    currentPropValues.Add(value.Key);

                fieldsValues.Add(property, currentPropValues);
            }

            return fieldsValues;
        }
    }
}