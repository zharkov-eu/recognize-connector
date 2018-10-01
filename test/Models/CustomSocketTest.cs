using Xunit;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class SocketFieldsProcessorTest
    {
        private readonly SocketFieldsProcessor _processor;

        public SocketFieldsProcessorTest()
        {
            _processor = new SocketFieldsProcessor();
        }

        public List<CustomSocket> GetSockets()
        {
            var csvSnapshot = "mpn;color;contact_material;contact_plating;gender;housing_color;housing_material;mounting_style;number_of_contacts;number_of_positions;number_of_rows;orientation;pin_pitch;material;size_diameter;size_length;size_height;size_width\n";
			csvSnapshot += "5145167-4;Natural;Phosphor Bronze;Gold;Female;Natural;Thermoplastic;Through Hole;120;60;2;Vertical;0.00127;Plastic;11.5;5.3;3.2;10.0\n";
			csvSnapshot += "1-329631-2;Silver;;;;;;;;;;;;;;;;\n";
            csvSnapshot += "1-5145154-2;White;Phosphor Bronze;Gold;Female;Natural;Thermoplastic;Solder;120;60;2;Vertical;0.00127;Thermoplastic;;;;\n";
            csvSnapshot += "R3MZ;;;Nickel;Male;;;Solder;3;3;;;;;0.024384;0.06985;;\n";
            csvSnapshot += "A3MB;Black;;Silver;Male;Black;;Solder;3;3;;;;;0.018796;0.070612;;\n";
            csvSnapshot += "211882-1;;;;Male;Gray;Thermoplastic;;4;4;;;;Thermoplastic;;;;\n";
            csvSnapshot += "5145154-4;White;Phosphor Bronze;Gold;Female;Natural;Thermoplastic;Solder;120;60;2;Straight;0.00127;Thermoplastic;;;;\n";
            csvSnapshot += "1-208657-1;Black;Phosphor Bronze;Gold;Female;Black;Nylon;Flange;8;8;;;;;;;;\n";
            csvSnapshot += "CR2-M;Natural;;;;;;;;;;;;Nylon;;0.0084;0.005;0.0051\n";
            csvSnapshot += "2106710-1;Natural;Phosphor Bronze;Tin;Female;Natural;;Screw;;;;;;;;;;\n";
            csvSnapshot += "MS3476W14-19P;;;Gold;Male;;;;19;;;;;;;;;\n";
            csvSnapshot += "C3M;Silver;;Nickel;Male;;;Panel;3;3;;;;;0.0206502;0.0256032;;\n";
            csvSnapshot += "8-215570-0;Red;Phosphor Bronze;Tin;Male;Red;Polyester;Through Hole;10;10;;Vertical;0.00127;Polyester;;;;\n";
            csvSnapshot += "D5M;Silver;Gold;Gold;Male;;;Panel;5;5;;;;;0.018923;0.0206502;;\n";
            csvSnapshot += "8-215570-2;Red;Phosphor Bronze;Tin;Male;Red;Polyester;Through Hole;12;12;;Vertical;0.00127;Polyester;;;;\n";
            csvSnapshot += "MS3476L22-55P;;;Gold;Male;;;;55;;;;;;;;;\n";
            csvSnapshot += "EE-1003A;;;;;;;Bracket;;;;;;;;;;\n";
            csvSnapshot += "5145154-1;Natural;Phosphor Bronze;Gold Flash;Female;Natural;Thermoplastic;Through Hole;120;;;Vertical;;;;;;\n";
            csvSnapshot += "84952-8;Natural;Phosphor Bronze;Tin;Female;White;;Surface Mount;8;;;Right Angle;0.001;Thermoplastic;;;;\n";
            csvSnapshot += "1-206455-2;Black;;Gold;Male;Black;Thermoplastic;Flange;63;63;;;;;;;;\n";
            csvSnapshot += "20ESRM-3;;;;Male;;;Panel;;;;;;;;;;\n";
            csvSnapshot += "CR4H-M0;Natural;;;;;;;;;;;;Nylon;;0.0146;0.0076;0.0091\n";
            csvSnapshot += "TA5FLX;;Copper Alloy;Tin;Male;;;Solder;5;;;;;;;;;\n";
            csvSnapshot += "6331G1;Red;Copper;Silver;Male;Red;;Solder;2;;;;;;;;;\n";

            List<CustomSocket> sockets;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvSnapshot)))
                return sockets = _processor.GetSockets(stream);
        }

        public Dictionary<string, List<string>> GetFieldsWithPossibleValues(List<CustomSocket> sockets)
        {
           return _processor.GetFieldsWithPossibleValues(sockets);
        }

        [Fact]
        public void SocketFieldsProcessor_GetSockets()
        {
            List<CustomSocket> sockets = GetSockets();
            CustomSocket socket = new CustomSocket();
            socket.Color = "Natural";
            socket.SocketName = "5145167-4";
            socket.ContactMaterial = "Phosphor Bronze";
            socket.ContactPlating = "Gold";
            socket.Gender = "Female";
            socket.HousingColor = "Natural";
            socket.HousingMaterial = "Thermoplastic";
            socket.MountingStyle = "Through Hole";
            socket.NumberOfContacts = 120;
            socket.NumberOfPositions = 60;
            socket.NumberOfRows = 2;
            socket.Orientation = "Vertical";
            socket.PinPitch = 0.00127f;
            socket.Material = "Plastic";
            socket.SizeDiameter = 11.5f;
            socket.SizeLength = 5.3f;
            socket.SizeHeight = 3.2f;
            socket.SizeWidth = 10.0f;

            Assert.True(socket.Equals(sockets[0]), "Данные разъема должны парситься правильно");
        }

        [Fact]
        public void SocketFieldsProcessor_GetFieldsWithPossibleValues()
        {

        }
    }
}