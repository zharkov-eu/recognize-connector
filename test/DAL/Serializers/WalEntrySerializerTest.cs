using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.DAL.Serializers;
using Xunit;

namespace ExpertSystem.Tests.DAL.Serializers
{
    public class WallEntrySerializerTest
    {
        // TODO: Перенести разъёмы в единый файл
        private readonly CustomSocket _socket;
        private readonly string _socketCsvLine;
        private readonly WalEntry<CustomSocket> _walEntryInsert;
        private readonly string _insertString;
        private readonly WalEntry<CustomSocket> _walEntryUpdate;
        private readonly string _updateString;
        private readonly WalEntry<CustomSocket> _walEntryDelete;
        private readonly string _deleteString;

        private readonly WalEntrySerializer<CustomSocket> _walSerializer;

        public WallEntrySerializerTest()
        {
            // Инициализация сериализатора для CustomSocket
            _walSerializer = new WalEntrySerializer<CustomSocket>(new CustomSocketSerializer());

            // Заполнение тестовых данных
            _socket = new CustomSocket
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
            _socketCsvLine =
                "5747871-8;Male;Brass;Tin;Black;Black;Thermoplastic;Through Hole;9;9;2;Vertical;0.002743;;;0.03081;0.008128;0.012548;";
            _walEntryInsert = new WalEntry<CustomSocket>(CsvDbAction.Insert, _socket.GetHashCode(), _socket);
            _insertString = $"Insert:::{_socket.GetHashCode()}:::{_socketCsvLine}";
            _walEntryUpdate = new WalEntry<CustomSocket>(CsvDbAction.Update, _socket.GetHashCode(), _socket);
            _updateString = $"Update:::{_socket.GetHashCode()}:::{_socketCsvLine}";
            _walEntryDelete = new WalEntry<CustomSocket>(CsvDbAction.Delete, _socket.GetHashCode());
            _deleteString = $"Delete:::{_socket.GetHashCode()}:::";
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