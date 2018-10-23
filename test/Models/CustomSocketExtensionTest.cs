using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using Xunit;
using Xunit.Abstractions;

namespace ExpertSystem.Tests.Models
{
    /// <summary>Класс тестирования методов StorageCustomSocket</summary>
    public class StorageCustomSocketTest
    {
        // Для вывода данных теста в консоль
        private readonly ITestOutputHelper _output;

        // Тестовые данные
        private readonly string _testLine;
        private readonly CustomSocket _testSocket;

        public StorageCustomSocketTest(ITestOutputHelper output)
        {
            _output = output;

            // Заполняем тестовые данные
            _testLine =
                "5747871-8;Male;Brass;Tin;Black;Black;Thermoplastic;Through Hole;9;9;2;Vertical;0.002743;;;0.03081;0.008128;0.012548;";
            _testSocket = new CustomSocket
            {
                SocketName = "5747871-8",
                Gender = "Male",
                ContactMaterial = "Brass",
                ContactPlating = "Tin",
                Color = "Black",
                HousingColor = "Black",
                HousingMaterial = "Thermoplastic",
                MountingStyle = "Through Hole",
                NumberOfContacts = 9,
                NumberOfPositions = 9,
                NumberOfRows = 2,
                Orientation = "Vertical",
                PinPitch = 0.002743f,
                Material = "",
                SizeDiameter = -1f,
                SizeLength = 0.03081f,
                SizeHeight = 0.008128f,
                SizeWidth = 0.012548f
            };
        }

        /// <summary>Проверка коректности серизации CustomSocket в CSV</summary>
        [Fact]
        public void Serialize_isCorrect()
        {
            // Arrange
            var inputSocket = _testSocket;
            var expectedLine = _testLine;

            // Act
            var actualLine = CustomSocketExtension.Serialize(inputSocket);

            // Assert
            Assert.Equal(expectedLine, actualLine);
        }

        /// <summary>Проверка десериализации CSV частей в CustomSocket</summary>
        [Fact]
        public void Deserialize_line_isCorrect()
        {
            // Arrange
            var inputLine = _testLine;
            var expectedSocket = _testSocket;

            // Act
            var actualSocket = CustomSocketExtension.Deserialize(inputLine);

            // Assert
            Assert.Equal(expectedSocket, actualSocket);
        }

        /// <summary>Проверка десериализации CSV строки в CustomSocket</summary>
        [Fact]
        public void Deserialize_parts_isCorrect()
        {
            // Arrange
            var inputParts = _testLine.Split(';');
            var expectedSocket = _testSocket;

            // Act
            var actualSocket = CustomSocketExtension.Deserialize(inputParts);

            // Assert
            Assert.Equal(expectedSocket, actualSocket);
        }
    }
}