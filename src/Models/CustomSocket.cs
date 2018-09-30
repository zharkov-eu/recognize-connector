using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using ExpertSystem.Parse;

namespace ExpertSystem.Models
{
    public class CustomSocket
    {
        public static readonly string[] DOMAINS =
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

        public string SocketName;
        public string Gender;
        public string ContactMaterial;
        public string ContactPlating;
        public string Color;
        public string HousingColor;
        public string HousingMaterial;
        public string MountingStyle;
        public int NumberOfContacts;
        public int NumberOfPositions;
        public int NumberOfRows;
        public string Orientation;
        public float PinPitch;
        public string Material;
        public float SizeDiameter;
        public float SizeLength;
        public float SizeHeight;
        public float SizeWidth;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var socket = obj as CustomSocket;
            if (socket == null)
                return false;
            return SocketName == socket.SocketName &&
                   Gender == socket.Gender &&
                   ContactMaterial == socket.ContactMaterial &&
                   ContactPlating == socket.ContactPlating &&
                   Color == socket.Color &&
                   HousingColor == socket.HousingColor &&
                   HousingMaterial == socket.HousingMaterial &&
                   MountingStyle == socket.MountingStyle &&
                   NumberOfContacts == socket.NumberOfContacts &&
                   NumberOfPositions == socket.NumberOfPositions &&
                   NumberOfRows == socket.NumberOfRows &&
                   Orientation == socket.Orientation &&
                   PinPitch == socket.PinPitch &&
                   Material == socket.Material &&
                   SizeDiameter == socket.SizeDiameter &&
                   SizeLength == socket.SizeLength &&
                   SizeHeight == socket.SizeHeight &&
                   SizeWidth == socket.SizeWidth;
        }

        public override int GetHashCode()
        {
            var hash = 19;
            hash += SocketName.GetHashCode();
            hash += Gender.GetHashCode();
            hash += ContactMaterial.GetHashCode();
            hash += ContactPlating.GetHashCode();
            hash += Color.GetHashCode();
            hash += HousingColor.GetHashCode();
            hash += HousingMaterial.GetHashCode();
            hash += MountingStyle.GetHashCode();
            hash += NumberOfContacts.GetHashCode();
            hash += NumberOfPositions.GetHashCode();
            hash += NumberOfRows.GetHashCode();
            hash += Orientation.GetHashCode();
            hash += PinPitch.GetHashCode();
            hash += Material.GetHashCode();
            hash += SizeDiameter.GetHashCode();
            hash += SizeLength.GetHashCode();
            hash += SizeHeight.GetHashCode();
            hash += SizeWidth.GetHashCode();
            return base.GetHashCode();
        }
    }

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
                            Color = line[1],
                            ContactMaterial = line[2],
                            ContactPlating = line[3],
                            Gender = line[4],
                            HousingColor = line[5],
                            HousingMaterial = line[6],
                            MountingStyle = line[7],
                            NumberOfContacts = string.IsNullOrEmpty(line[8]) ? -1 : int.Parse(line[8]),
                            NumberOfPositions = string.IsNullOrEmpty(line[9]) ? -1 : int.Parse(line[9]),
                            NumberOfRows = string.IsNullOrEmpty(line[10]) ? -1 : int.Parse(line[10]),
                            Orientation = line[11],
                            PinPitch = string.IsNullOrEmpty(line[12]) ? -1.0f : float.Parse(line[12], CultureInfo.InvariantCulture),
                            Material = line[13],
                            SizeDiameter = string.IsNullOrEmpty(line[14]) ? -1.0f : float.Parse(line[14], CultureInfo.InvariantCulture),
                            SizeLength = string.IsNullOrEmpty(line[15]) ? -1.0f : float.Parse(line[15], CultureInfo.InvariantCulture),
                            SizeHeight = string.IsNullOrEmpty(line[16]) ? -1.0f : float.Parse(line[16], CultureInfo.InvariantCulture),
                            SizeWidth = string.IsNullOrEmpty(line[17]) ? -1.0f : float.Parse(line[17], CultureInfo.InvariantCulture)
                        });
            }

            return entries;
        }

        public Dictionary<string, List<string>> GetFieldsWithPossibleValues(List<CustomSocket> sockets)
        {
            var fieldsValues = new Dictionary<string, List<string>>();
            var customSocketType = typeof(CustomSocket);
            foreach (var property in CustomSocket.DOMAINS)
            {
                var field = customSocketType.GetField(property);

                var propertyValues = sockets.GroupBy(p => field.GetValue(p).ToString()).ToList();
                var currentPropValues = new List<string>();
                foreach (var value in propertyValues)
                    currentPropValues.Add(value.Key);

                fieldsValues.Add(property, currentPropValues);
            }

            return fieldsValues;
        }
    }
}