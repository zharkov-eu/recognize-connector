using Xunit;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.DAL.Serializers;
using ExpertSystem.Tests.Configuration;

namespace ExpertSystem.Tests.DAL.Serializers
{
    public class WallEntrySerializerTest
    {
        // Тестовые данные
        private readonly CustomSocket _socket = TestData.GetSocket();
        private readonly string _socketCsvLine = TestData.GetSocketCsvLine();
        private readonly WalEntry<CustomSocket> _walEntryInsert;
        private readonly string _insertString;
        private readonly WalEntry<CustomSocket> _walEntryUpdate;
        private readonly string _updateString;
        private readonly WalEntry<CustomSocket> _walEntryDelete;
        private readonly string _deleteString;

        // Мок объект сериализатора
        private readonly WalEntrySerializer<CustomSocket> _walSerializer;

        public WallEntrySerializerTest()
        {
            // Инициализация сериализатора для CustomSocket
            _walSerializer = new WalEntrySerializer<CustomSocket>(new CustomSocketSerializer());

            // Заполнение тестовых данных
            _walEntryInsert = new WalEntry<CustomSocket>(CsvDbAction.Insert, _socket.GetHashCode(), _socket);
            _insertString = $"Insert:::{_socket.GetHashCode()}:::{_socketCsvLine}";
            _walEntryUpdate = new WalEntry<CustomSocket>(CsvDbAction.Update, _socket.GetHashCode(), _socket);
            _updateString = $"Update:::{_socket.GetHashCode()}:::{_socketCsvLine}";
            _walEntryDelete = new WalEntry<CustomSocket>(CsvDbAction.Delete, _socket.GetHashCode(), _socket);
            _deleteString = $"Delete:::{_socket.GetHashCode()}:::{_socketCsvLine}";
        }

        /// <summary>Проверка правильности преобразования WAL записи вставки в строку</summary>
        [Fact]
        public void Serialize_Insert_isCorrect()
        {
            // Arrange
            var expectedString = _insertString;

            // Act
            var actualString = _walSerializer.Serialize(_walEntryInsert);

            // Assert
            Assert.Equal(expectedString, actualString);
        }

        /// <summary>Проверка правильности преобразования WAL записи обновления в строку</summary>
        [Fact]
        public void Serialize_Update_isCorrect()
        {
            // Arrange
            var expectedString = _updateString;

            // Act
            var actualString = _walSerializer.Serialize(_walEntryUpdate);

            // Assert
            Assert.Equal(expectedString, actualString);
        }

        /// <summary>Проверка правильности преобразования WAL записи удаления в строку</summary>
        [Fact]
        public void Serialize_Delete_isCorrect()
        {
            // Arrange
            var expectedString = _deleteString;

            // Act
            var actualString = _walSerializer.Serialize(_walEntryDelete);

            // Assert
            Assert.Equal(expectedString, actualString);
        }

        /// <summary>Проверка правильности преобразование строки вставки в WAL запись</summary>
        [Fact]
        public void Deserialize_Insert_isCorrect()
        {
            // Arrange
            var expectedWal = _walEntryInsert;

            // Act
            var actualWal = _walSerializer.Deserialize(_insertString);

            // Assert
            Assert.Equal(expectedWal, actualWal);
        }
    }
}