using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpertSystem.Parse;

namespace ExpertSystem.Models
{
    public class CustomSocket {

        public string SocketName {get; set; }
        public string gender;
        public string contact_material;
        public string contact_plating;
        public string color;
        public string housing_color;
        public string housing_material;
        public string mounting_style;
        public string number_of_contacts;
        public string number_of_positions;
        public string number_of_rows;
        public string orientation;
        public string pin_pitch;
        public string material;
        public string size_diameter;
        public string size_length;
        public string size_height;
        public string size_width;         
    }

    public class SocketFieldsProcessor
    {
        private static readonly string[] ORDER_BY =
        {
            "gender",
            "contact_material",
            "contact_plating",
            "color",
            "housing_color",
            "housing_material",
            "mounting_style",
            "number_of_contacts",
            "number_of_positions",
            "number_of_rows",
            "orientation",
            "pin_pitch",
            "material",
            "size_diameter",
            "size_length",
            "size_height",
            "size_width"
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
                            color = line[1],
                            contact_material = line[2],
                            contact_plating = line[3],
                            gender = line[4],
                            housing_color = line[5],
                            housing_material = line[6],
                            number_of_contacts = line[7],
                            number_of_positions = line[8],
                            number_of_rows = line[9],
                            orientation = line[10],
                            pin_pitch = line[11],
                            material = line[12],
                            size_diameter = line[13],
                            size_length = line[14],
                            size_height = line[15],
                            size_width = line[16]
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