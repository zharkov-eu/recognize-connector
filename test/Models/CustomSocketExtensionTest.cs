using Xunit;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using ExpertSystem.Tests.Configuration;

namespace ExpertSystem.Tests.Models
{
    /// <summary>Класс тестирования методов StorageCustomSocket</summary>
    public class CustomSocketExtensionTest
    {
        // Тестовые данные
        private readonly string _testLine = TestData.GetSocketCsvLine();
        private readonly CustomSocket _testSocket = TestData.GetSocket();

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