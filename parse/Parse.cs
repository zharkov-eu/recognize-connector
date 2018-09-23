using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

class Sort {
    private static string[] ORDER_BY = {
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
        "size_width",
    };

    public void parse() {
        List<CustomSocket> entries = new List<CustomSocket>();
        Type customSocketType = typeof(CustomSocket);
        foreach (string property in ORDER_BY) {
            var group = entries.GroupBy(p => {
                customSocketType.GetField(property).GetValue(p);
            });
        }
    }
}