using System.Collections.Generic;
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
            var entries = new List<CustomSocket>();
            var customSocketType = typeof(CustomSocket);
            foreach (var property in ORDER_BY)
            {
                var propertyValues = entries.GroupBy(p => customSocketType.GetField(property).GetValue(p)).ToList();
            }
        }
    }
}