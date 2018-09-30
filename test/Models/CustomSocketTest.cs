using Xunit;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ExpertSystem.Models
{
    public class SocketFieldsProcessor_isCorrect
    {
        private readonly CustomSocket _socket;
        private readonly SocketFieldsProcessor _processor;

        public SocketFieldsProcessor_isCorrect()
        {
            _socket = new CustomSocket();
            _processor = new SocketFieldsProcessor();

            _socket.Color = "Natural";
            _socket.SocketName = "5145167-4";
            _socket.ContactMaterial = "Phosphor Bronze";
            _socket.ContactPlating = "Gold";
            _socket.Gender = "Female";
            _socket.HousingColor = "Natural";
            _socket.HousingMaterial = "Thermoplastic";
            _socket.MountingStyle = "Through Hole";
            _socket.NumberOfContacts = 120;
            _socket.NumberOfPositions = 60;
            _socket.NumberOfRows = 2;
            _socket.Orientation = "Vertical";
            _socket.PinPitch = 0.00127f;
            _socket.Material = "Plastic";
            _socket.SizeDiameter = 11.5f;
            _socket.SizeLength = 5.3f;
            _socket.SizeHeight = 3.2f;
            _socket.SizeWidth = 10.0f;
        }

        [Fact]
        public void SocketFieldsProcessor_GetSockets()
        {
            var csvSnapshot = "mpn;color;contact_material;contact_plating;gender;housing_color;housing_material;mounting_style;number_of_contacts;" +
                              "number_of_positions;number_of_rows;orientation;pin_pitch;material;size_diameter;size_length;size_height;size_width\n";
            csvSnapshot += "5145167-4;Natural;Phosphor Bronze;Gold;Female;Natural;Thermoplastic;Through Hole;" +
                           "120;60;2;Vertical;0.00127;Plastic;11.5;5.3;3.2;10.0";

            List<CustomSocket> sockets;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvSnapshot)))
                sockets = _processor.GetSockets(stream);

            Assert.True(_socket.Equals(sockets[0]), "Данные разъема должны парситься правильно");
        }

        [Fact]
        public void SocketFieldsProcessor_GetFieldsWithPossibleValues()
        {

        }
    }
}