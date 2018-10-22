using Xunit;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.Parsers;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Tests.Parsers
{
    public class SocketParserTest
    {
        public CustomSocket TestSocket()
        {
            return new CustomSocket
            {
                Color = "Natural",
                SocketName = "5145167-4",
                ContactMaterial = "Phosphor Bronze",
                ContactPlating = "Gold",
                Gender = "Female",
                HousingColor = "Natural",
                HousingMaterial = "Thermoplastic",
                MountingStyle = "Through Hole",
                NumberOfContacts = 120,
                NumberOfPositions = 60,
                NumberOfRows = 2,
                Orientation = "Vertical",
                PinPitch = 0.00127f,
                Material = "Plastic",
                SizeDiameter = 0.015f,
                SizeLength = 0.0145f,
                SizeHeight = 0.00415f,
                SizeWidth = 0.0034f
            };
        }

        public List<CustomSocket> GetSockets()
        {
            var csvSnapshot =
                "mpn;gender;contact_material;contact_plating;color;housing_color;housing_material;mounting_style;number_of_contacts;number_of_positions;number_of_rows;orientation;pin_pitch;material;size_diameter;size_length;size_height;size_width\n";
            csvSnapshot +=
                "5145167-4;Female;Phosphor Bronze;Gold;Natural;Natural;Thermoplastic;Through Hole;120;60;2;Vertical;0.00127;Plastic;0.015;0.0145;0.00415;0.0034\n";
            csvSnapshot +=
                "XF2J-2024-11A;Female;Copper Alloy;Gold;;;Nylon 4/6;Surface Mount;20;20;1;;0.0005;;;0.0145;0.00415;0.0034\n";
            csvSnapshot += 
                "3-644540-7;Female;Copper Alloy;Tin;Red;Red;Nylon;Through Hole;7;7;1;;0.00254;;;0.01778;0.013208;0.007747\n";
            csvSnapshot += 
                "3-640428-7;Female;Copper Alloy;Tin;Red;Red;Nylon;;7;7;1;;0.00396;;;0.027737;0.018415;0.009017\n";
            csvSnapshot += 
                "487526-1;Male;;;Black;Black;Thermoplastic;;2;2;;;0.00254;;;0.00492;0.01371;0.00248\n";
            csvSnapshot += 
                "1445957-4;;;Tin;Green;Green;Polycarbonate;;8;1;;;;;;0.007899;0.024587;0.007899\n";
            csvSnapshot += 
                "54489-3;Male;;;Natural;Natural;Nylon;Panel;3;3;1;;0.00792;;;0.023774;0.024511;0.007925\n";
            csvSnapshot += 
                "4-640443-0;Female;Copper Alloy;Tin;Green;Green;Nylon;;10;10;1;;0.00254;;;0.0254;0.013208;0.006985\n";
            csvSnapshot += 
                "XF2L-0425-1A;Female;Copper Alloy;Gold;;;;Surface Mount;4;4;;;0.0005;;;0.0069;0.0012;0.00345\n";
            csvSnapshot += 
                "640441-4;Female;Copper Alloy;Tin Lead;White;White;Nylon;;4;4;1;;0.00254;;;0.01016;0.013208;0.006985\n";
            csvSnapshot += 
                "3-640599-6;Female;Copper Alloy;Tin;Orange;Orange;Nylon;;6;6;1;;0.00396;;;0.023774;0.018415;0.009779\n";
            csvSnapshot += 
                "XF2J-2624-11A;Female;;Gold;;;;Surface Mount;26;26;;;0.0005;;;0.0175;0.00415;0.0034\n";
            csvSnapshot += 
                "74441-0017;Female;Copper Alloy;Tin;Black;Black;Plastic;Surface Mount;30;30;2;Right Angle;8,00E-05;Thermoplastic;;0.015;0.00537;0.00795\n";
            csvSnapshot += 
                "3-643817-3;Female;Copper Alloy;Tin;Orange;Orange;Nylon;;3;3;1;;0.00396;;;0.011887;0.018415;0.009017\n";
            csvSnapshot += 
                "3-643819-5;Female;Copper Alloy;Tin;Red;Red;Nylon;;5;5;1;;0.00396;;;0.019812;0.018415;0.009017\n";
            csvSnapshot += 
                "3-640426-7;Female;Copper Alloy;Tin;Orange;Orange;Nylon;Through Hole;7;7;1;;0.00396;;;0.027737;0.018415;0.009017\n";
            csvSnapshot += 
                "640442-3;;Copper Alloy;Tin Lead;Blue;White;Nylon;;3;3;1;;0.00254;;;0.00762;0.013208;0.006985\n";
            csvSnapshot += 
                "1-111626-9;Female;Beryllium Copper;Tin Lead;Black;Black;Plastic;;20;20;2;;0.002;;;0.02517;0.010236;0.005156\n";
            csvSnapshot += 
                "640442-2;Female;Copper Alloy;Tin Lead;Blue;Blue;Nylon;PCB;2;2;1;;0.00254;;;0.00508;0.013208;0.006985\n";
            csvSnapshot += 
                "640441-6;Female;Copper Alloy;Tin Lead;;White;Nylon;;6;6;1;;0.00254;;;0.01524;0.013208;0.006985\n";
            csvSnapshot += 
                "3-641535-2;Female;Copper Alloy;Tin;White;White;Nylon;PC Board;2;2;1;;0.00254;;;0.00508;0.013462;0.006985\n";


            List<CustomSocket> sockets;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvSnapshot)))
                return sockets = SocketParser.ParseSockets(new StreamReader(stream));
        }

        [Fact]
        public void SocketFieldsProcessor_GetSockets()
        {
            var sockets = GetSockets();
            var socket = TestSocket();

            Assert.True(socket.Equals(sockets[0]), "Данные разъема должны парситься правильно");
        }
    }
}