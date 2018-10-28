using Xunit;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using ExpertSystem.Tests.Configuration;

namespace ExpertSystem.Tests.Models
{
    /// <summary>Класс для тестирования методов SocketGroupExtension</summary>
    public class SocketGroupExtensionTest
    {
        // Тестовые данные
        private readonly string _line = TestData.GetSocketGroupCsvLine();
        private readonly string[] _parts = TestData.GetSocketGroupCsvLine().Split(SocketGroupExtension.Delimiter);
        private readonly SocketGroup _group = TestData.GetSocketGroup();

        /// <summary>Проверка десериализации CSV частей в SocketGroup</summary>
        [Fact]
        public void Serialize_isCorrect()
        {
            // Arrange
            var expectedLine = _line;

            // Act
            var actualLine = SocketGroupExtension.Serialize(_group);

            // Assert
            Assert.Equal(expectedLine, actualLine);
        }

        /// <summary>Проверка десериализации CSV строки в SocketGroup</summary>
        [Fact]
        public void Deserialize_line_isCorrect()
        {
            // Arrange
            var expectedGroup = _group;

            // Act
            var actualGroup = SocketGroupExtension.Deserialize(_line);

            // Assert
            Assert.Equal(expectedGroup, actualGroup);
        }

        /// <summary>Проверка десериализации CSV частей в SocketGroup</summary>
        [Fact]
        public void Deserialize_parts_isCorrect()
        {
            // Arrange
            var expectedGroup = _group;

            // Act
            var actualGroup = SocketGroupExtension.Deserialize(_parts);

            // Assert
            Assert.Equal(expectedGroup, actualGroup);
        }
    }
}