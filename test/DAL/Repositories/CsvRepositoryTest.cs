using Xunit;
using System;
using System.IO;
using System.Text;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Parsers;
using ExpertSystem.Common.Serializers;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.DAL.Repositories;
using ExpertSystem.Server.DAL.Serializers;
using ExpertSystem.Tests.Configuration;

namespace ExpertSystem.Tests.DAL.Repositories
{
    /// <inheritdoc />
    /// <summary>Класс тестирования методов CsvRepository</summary>
    public class CsvRepositoryTest : IDisposable
    {
        // Путь до тестовой директории
        private readonly string _testDir;

        // Путь до тестового CSV файла
        private readonly string _testCsvFileName;

        // Путь до тестового WAL файла
        private readonly string _testWalFileName;

        // Мок репозитория
        private readonly CsvRepository<CustomSocket> _repositoryMock;

        // Сериализатор WAL-записей
        private readonly WalEntrySerializer<CustomSocket> _walSerializer 
            = new WalEntrySerializer<CustomSocket>(new CustomSocketSerializer());
        
        // Тестовые данные
        private readonly string _testCsvData = TestData.GetSocketsCsv();
        private readonly CustomSocket _testSocket = TestData.GetSocket();

        public CsvRepositoryTest()
        {
            // Определение пути тестовой директории
            _testDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ExpertSystem");
            // Создание тестовой директории
            Directory.CreateDirectory(_testDir);
            // Определение имени тестового CSV файла
            _testCsvFileName = Path.Combine(_testDir, "csvTest.csv");
            // Определение имени тестового WAL файла
            _testWalFileName = Path.Combine(_testDir, "walTest.txt");
            // Обновление тестовых CSV данных
            UpdateTestCsvData();
            // Создание мока репозитория
            var serializer = new CustomSocketSerializer();
            var parser = new CsvRecordParser<CustomSocket>(serializer);
            var options = new CsvRepositoryOptions
            {
                CsvFileName =  _testCsvFileName,
                WalFileName = _testWalFileName,
                IdPropertyName = "SocketName"
            };
            _repositoryMock = new CsvRepository<CustomSocket>(serializer, parser, options);
            // Синхронизируем репозиторий
            UpdateTestWalData();
            _repositoryMock.Sync();
        }

        /// <summary>Обновление данных CSV файла</summary>
        private void UpdateTestCsvData()
        {
            FileUtils.CreateFileAndFill(_testCsvFileName, _testCsvData);
        }

        /// <summary>Обновление данных WAL файла</summary>
        private void UpdateTestWalData()
        {
            FileUtils.CreateFileAndFill(_testWalFileName, "");
        }

        /// <summary>Обновление данных тестовых файлов</summary>
        private void UpdateTestData()
        {
            UpdateTestCsvData();
            UpdateTestWalData();
        }

        /// <summary>Проверка верности синхронизации</summary>
        [Fact]
        public void Sync_isCorrect()
        {
            // Arrange

            // Act
            _repositoryMock.Sync();

            // Assert
            Assert.True(GetNumLinesInFile(_testCsvFileName, 1).Equals(_repositoryMock.GetAllRecords().Count), 
                "Новое число записей в CSV должно равнять числу разъёмов в памяти");
            Assert.True(File.Exists(_testWalFileName), 
                "WAL файл не существует");
            Assert.True(new FileInfo(_testWalFileName).Length == 0, 
                "WAL файл не пуст");
        }

        /// <summary>Проверка верности синхронизации действия вставки</summary>
        [Fact]
        public void Sync_Insert_isCorrect()
        {
            // Arrange
            var socketToInsert = _testSocket.Clone();
            socketToInsert.SocketName = socketToInsert.SocketName + "#";

            // Act
            _repositoryMock.Insert(socketToInsert);
            _repositoryMock.Sync();

            //Assert
            // CSV файл обновлён
            Assert.True(File.ReadAllText(_testCsvFileName, Encoding.UTF8).Contains(socketToInsert.SocketName),
                "Вставленный разъём не присутствует в CSV файле");
            // WAL файл пуст
            Assert.True(new FileInfo(_testWalFileName).Length == 0,
                "WAL файл пуст");

            // Clear
            UpdateTestData();
        }

        /// <summary>Проверка верности синхронизации действия обновления</summary>
        [Fact]
        public void Sync_Update_isCorrect()
        {
            // Arrange
            var updatedSocket = _testSocket.Clone();
            var hashCode = updatedSocket.GetHashCode();
            updatedSocket.MountingStyle = "Through hole";

            // Act
            _repositoryMock.Update(hashCode, updatedSocket);
            _repositoryMock.Sync();

            //Assert
            // CSV файл обновлён
            Assert.True(File.ReadAllText(_testCsvFileName, Encoding.UTF8).Contains(updatedSocket.SocketName),
                "Удалённый разъём не присутствует в CSV файле");
            // WAL файл пуст
            Assert.True(new FileInfo(_testWalFileName).Length == 0,
                "WAL файл не пуст");

            // Clear
            UpdateTestData();
        }

        /// <summary>Проверка верности синхронизации действия удаления</summary>
        [Fact]
        public void Sync_Delete_isCorrect()
        {
            // Arrange
            var socketToDelete = _testSocket.Clone();

            // Act
            _repositoryMock.Delete(socketToDelete.SocketName);
            _repositoryMock.Sync();

            //Assert
            // CSV файл обновлён
            Assert.True(!File.ReadAllText(_testCsvFileName, Encoding.UTF8).Contains(socketToDelete.SocketName),
                "Удалённый разъём найден в CSV файле");
            // WAL файл пуст
            Assert.True(new FileInfo(_testWalFileName).Length == 0,
                "WAL файл не пуст");

            // Clear
            UpdateTestData();
        }

        /// <summary>Проверка верности получения списка разъёмов</summary>
        [Fact]
        public void GetSockets_isCorrect()
        {
            // Arrange
            _repositoryMock.Sync();

            // Act
            var actualSockets = _repositoryMock.GetAllRecords();

            // Assert
            Assert.True(GetNumLines(_testCsvData, 1).Equals(actualSockets.Count), 
                "Полученное число разъёмов не соответствует ожидаемому числу");
        }

        /// <summary>Проверка правильности выполнения выборки</summary>
        [Fact]
        public void Select_isCorrect()
        {
            // Arrange
            var socketName = _testSocket.SocketName;

            // Act
            var actualSocket = _repositoryMock.Select(socketName).Item2;

            // Assert
            Assert.True(socketName.Equals(actualSocket.SocketName), 
                "Полученный разъём не соответствует ожидаемому по имени");
        }

        /// <summary>Проверка правильности выполнения вставки</summary>
        [Fact]
        public void Insert_isCorrect()
        {
            // Arrange
            var socketToInsert = _testSocket.Clone();
            socketToInsert.SocketName = socketToInsert.SocketName + "#";
            var expectedWalEntryLine = _walSerializer.Serialize(
                new WalEntry<CustomSocket>(CsvDbAction.Insert, socketToInsert.GetHashCode(), socketToInsert));

            // Act
            _repositoryMock.Insert(socketToInsert);

            // Assert
            Assert.True(_repositoryMock.GetAllRecords().Contains(socketToInsert),
                "Ожидаемый разъём не найден в репозитории");
            Assert.True(File.ReadAllText(_testWalFileName, Encoding.UTF8).Contains(expectedWalEntryLine),
                "Запись о вставке разъёма не присутствует в WAL файле");

            // Clear
            UpdateTestData();
        }

        /// <summary>Проверка правильности выполнения обновления</summary>
        [Fact]
        public void Update_isCorrect()
        {
            // Arrange
            var updatedSocket = _testSocket.Clone();
            var hashCode = updatedSocket.GetHashCode();
            updatedSocket.MountingStyle = "Through hole";
            var expectedWalLine = _walSerializer.Serialize(
                new WalEntry<CustomSocket>(CsvDbAction.Update, updatedSocket.GetHashCode(), updatedSocket));

            // Act
            _repositoryMock.Update(hashCode, updatedSocket);

            // Assert
            Assert.True(_repositoryMock.GetAllRecords().Contains(updatedSocket),
                "Обновлённый разъём не найден среди доступных");
            // TODO: Проверить WAL файл
//            Assert.True(File.ReadAllText(_testWalFileName, Encoding.UTF8).Contains(expectedWalLine),
//                "Запись об обновлении разъёма не присутствует в WAL файле");

            // Clear
            UpdateTestData();
        }

        /// <summary>Проверка правильности выполнения удаления</summary>
        [Fact]
        public void Delete_isCorrect()
        {
            // Arrange
            var socketToDelete = _testSocket.Clone();
            var expectedWalLine = _walSerializer.Serialize(
                new WalEntry<CustomSocket>(CsvDbAction.Delete, socketToDelete.GetHashCode(), socketToDelete));

            // Act
            _repositoryMock.Delete(socketToDelete.SocketName);

            // Assert
            Assert.True(!_repositoryMock.GetAllRecords().Contains(socketToDelete),
                "Удалённый разъём найден среди доступных");
            Assert.True(File.ReadAllText(_testWalFileName, Encoding.UTF8).Contains(expectedWalLine),
                "Запись об удалении разъёма не присутствует в WAL файле");

            // Clear
            UpdateTestData();
        }

        /// <inheritdoc />
        /// <summary>Действия по окончании всех тестов</summary>
        public void Dispose()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
        }

        /// <summary>Получить число линий (строк) в файле</summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="extra">Число лишних строк</param>
        /// <returns>Число линий в файле</returns>
        private static int GetNumLinesInFile(string fileName, int extra)
        {
            return GetNumLines(File.ReadAllText(fileName, Encoding.UTF8), extra);
        }

        /// <summary>Получить число линий в строке</summary>
        /// <param name="data">Срока</param>
        /// <param name="extra">Смещение</param>
        /// <returns>Число линий в строке</returns>
        private static int GetNumLines(string data, int extra)
        {
            if (data.Substring(data.Length - 1) == "\n")
                extra++;
            var numLines = data.Split('\n').Length - extra;
            return Math.Max(0, numLines);
        }
    }

    public static class FileUtils
    {
        /// <summary>Создать файл и заполнить данными</summary>
        /// <param name="path">Путь до файла</param>
        /// <param name="data">Данные</param>
        public static void CreateFileAndFill(string path, string data)
        {
            // Удалить файл если существует 
            if (File.Exists(path))
                File.Delete(path);
            // Создать файл
            using (var fs = File.Create(path))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(data);
                fs.Write(info, 0, info.Length);
            }
        }
    }
}