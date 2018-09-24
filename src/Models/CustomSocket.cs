using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpertSystem.Parse;

namespace ExpertSystem.Models
{
    public class CustomSocket {
        public string SocketName;
        public string Gender;
        public string ContactMaterial;
        public string ContactPlating;
        public string Color;
        public string HousingColor;
        public string HousingMaterial;
        public string MountingStyle;
        public string NumberOfContacts;
        public string NumberOfPositions;
        public string NumberOfRows;
        public string Orientation;
        public string PinPitch;
        public string Material;
        public string SizeDiameter;
        public string SizeLength;
        public string SizeHeight;
        public string SizeWidth;         
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

        public List<CustomSocket> GetSockets()
        {
            var fileName = Directory.GetCurrentDirectory() + "\\data\\1.csv";

            var entries = new List<CustomSocket>();
            using (var stream = File.OpenRead(fileName))
            using (var reader = new StreamReader(stream))
            {
                var data = CsvParser.ParseHeadAndTail(reader, ';', '"');
                var lines = data.Item2;
            
                foreach (var line in lines)
                {
                    if (line != null && line.Any())
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
                            MountingStyle = line[7],
                            NumberOfContacts = line[8],
                            NumberOfPositions = line[9],
                            NumberOfRows = line[10],
                            Orientation = line[11],
                            PinPitch = line[12],
                            Material = line[13],
                            SizeDiameter = line[14],
                            SizeLength = line[15],
                            SizeHeight = line[16],
                            SizeWidth = line[17]
                        });
                    }                  
                }
            }

            return entries;
        }

        public Dictionary<string, List<string>> GetFieldsWithPossibleValues(List<CustomSocket> sockets)
        {
            var fieldsValues = new Dictionary<string, List<string>>();
            var customSocketType = typeof(CustomSocket);
            foreach (var property in ORDER_BY)
            {
                var field = customSocketType.GetField(property);
                
                var propertyValues = sockets.GroupBy(p => (string)field.GetValue(p)).ToList();
                var currentPropValues = new List<string>();
                foreach (var value in propertyValues)
                    currentPropValues.Add(value.Key);

                fieldsValues.Add(property, currentPropValues);
            }

            return fieldsValues;
        }
    }
}