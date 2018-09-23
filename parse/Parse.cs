using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpertSystem.Models;

namespace ExpertSystem.Parse
{
    public class Sort
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

        public void Parse()
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

            var customSocketType = typeof(CustomSocket);



            foreach (var property in ORDER_BY)
            {
                var propertyValues = entries.GroupBy(p => customSocketType.GetField(property).GetValue(p)).ToList();
            }
        }
    }
}