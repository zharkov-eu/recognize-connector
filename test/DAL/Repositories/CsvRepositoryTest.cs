using Xunit;
using System;
using System.IO;
using System.Text;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using ExpertSystem.Common.Parsers;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.DAL.Repositories;
using ExpertSystem.Server.DAL.Serializers;

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

        // Тестовые данные в формате CSV
        private readonly string _testCsvData;

        // Мок репозитория
        private readonly CsvRepository<CustomSocket> _repositoryMock;

        // Тестовые данные
        private readonly CustomSocket _testSocket;

        public CsvRepositoryTest()
        {
            // Заполнение тестовых данных
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

            _testCsvData =
                "mpn;gender;contact_material;contact_plating;color;housing_color;housing_material;mounting_style;number_of_contacts;number_of_positions;number_of_rows;orientation;pin_pitch;material;size_diameter;size_length;size_height;size_width\n" +
                "3-640428-7;Female;Copper Alloy;Tin;Red;Red;Nylon;;7;7;1;;0.00396;;;0.027737;0.018415;0.009017\n" +
                "3-640599-6;Female;Copper Alloy;Tin;Orange;Orange;Nylon;;6;6;1;;0.00396;;;0.023774;0.018415;0.009779\n" +
                "3-644540-7;Female;Copper Alloy;Tin;Red;Red;Nylon;Through Hole;7;7;1;;0.00254;;;0.01778;0.013208;0.007747\n" +
                "3-643816-5;Female;Copper Alloy;Tin;Green;Green;Nylon;Through Hole;5;5;1;;0.00254;;;0.0127;0.013208;0.006985\n" +
                "3-640426-7;Female;Copper Alloy;Tin;Orange;Orange;Nylon;Through Hole;7;7;1;;0.00396;;;0.027737;0.018415;0.009017\n" +
                "3-643819-5;Female;Copper Alloy;Tin;Red;Red;Nylon;;5;5;1;;0.00396;;;0.019812;0.018415;0.009017\n" +
                "4-643817-0;Female;Copper Alloy;Tin;Orange;Orange;Nylon;;10;10;1;;0.00396;;;0.039624;0.009017;0.018415\n" +
                "640440-2;Female;Copper Alloy;Tin Lead;Red;Red;Nylon;;2;2;1;;0.00254;;;0.00508;0.013208;0.006985\n" +
                "1-640440-2;Female;Copper Alloy;Tin Lead;;Red;Nylon;;12;12;1;;0.00254;;;0.03048;0.013208;0.006985\n" +
                "3-641535-2;Female;Copper Alloy;Tin;White;White;Nylon;PC Board;2;2;1;;0.00254;;;0.00508;0.013462;0.006985\n" +
                "84953-4;Female;Phosphor Bronze;Tin;Natural;White;Plastic;Surface Mount;4;4;;Right Angle;0.001;Thermoplastic;;0.01192;0.00256;0.0054\n";

            // Определение пути тестовой директории
            _testDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ExpertSystem");
            // Создание тестовой директории
            Directory.CreateDirectory(_testDir);
            // Определение имени тестового CSV файла
            _testCsvFileName = Path.Combine(_testDir, "csvTest.csv");
            // Обновление данных CSV файла
            UpdateTestData();
            // Определение имени тестового WAL файла
            _testWalFileName = Path.Combine(_testDir, "walTest.txt");
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
            _repositoryMock.Sync();
        }

        /// <summary>Обновление данных CSV файла</summary>
        private void UpdateTestData()
        {
            // Создание и заполнение CSV файла
            FileUtils.CreateFileAndFill(_testCsvFileName, _testCsvData);
        }

        /// <summary>Проверка верности синхронизации</summary>
        [Fact]
        public void Sync_isCorrect()
        {
            // Arrange
            UpdateTestData();

            // Act
            _repositoryMock.Sync();

            // Assert
            Assert.Equal(GetNumLinesInFile(_testCsvFileName, 1), _repositoryMock.GetAllRecords().Count);
            Assert.True(File.Exists(_testWalFileName), "WAL файл не создан");
            Assert.True(new FileInfo(_testWalFileName).Length == 0, "WAL файл не пуст");
        }

        /// <summary>Проверка верности синхронизации действия вставки</summary>
        [Fact]
        public void Sync_Insert_isCorrect()
        {
            // Arrange
            UpdateTestData();
            var insertedSocket = _testSocket;

            // Act
            _repositoryMock.Insert(insertedSocket);
            _repositoryMock.Sync();

            //Assert
            // CSV файл обновлён
            Assert.True(File.ReadAllText(_testCsvData, Encoding.UTF8).Contains(insertedSocket.SocketName),
                "Вставленный разъём не присутсвует в CSV файле");
            // WAL файл пуст
            Assert.True(new FileInfo(_testWalFileName).Length == 0,
                "WAL файл не пуст");
        }

        /// <summary>Проверка верности синхронизации действия обновления</summary>
        [Fact]
        public void Sync_Update_isCorrect()
        {
            // Arrange
            var originalSocket = _repositoryMock.GetAllRecords()[0];
            var hashCode = originalSocket.GetHashCode();
            var updatedSocket = originalSocket;
            updatedSocket.MountingStyle = "Through hole";

            // Act
            _repositoryMock.Update(hashCode, updatedSocket);
            _repositoryMock.Sync();

            //Assert
            // CSV файл обновлён
            Assert.True(File.ReadAllText(_testCsvData, Encoding.UTF8).Contains(updatedSocket.SocketName),
                "Удалённый разъём не присутсвует в CSV файле");
            // WAL файл пуст
            Assert.True(new FileInfo(_testWalFileName).Length == 0,
                "WAL файл не пуст");
        }

        /// <summary>Проверка верности синхронизации действия удаления</summary>
        [Fact]
        public void Sync_Delete_isCorrect()
        {
            // Arrange
            var socketToDelete = _repositoryMock.GetAllRecords()[0];
            var socketToDeleteHashCode = socketToDelete.GetHashCode();

            // Act
//            _repositoryMock.Delete(socketToDeleteHashCode);
            _repositoryMock.Sync();

            //Assert
            // CSV файл обновлён
            Assert.True(!File.ReadAllText(_testCsvData, Encoding.UTF8).Contains(socketToDelete.SocketName),
                "Удалённый разъём всё ещё в CSV файле");
            // WAL файл пуст
            Assert.True(new FileInfo(_testWalFileName).Length == 0,
                "WAL файл не пуст");
        }

        /// <summary>Проверка верности получения списка разъёмов</summary>
        [Fact]
        public void GetSockets_isCorrect()
        {
            // Arrange
            FileUtils.CreateFileAndFill(_testCsvFileName, _testCsvData);
            _repositoryMock.Sync();

            // Act
            var actualSockets = _repositoryMock.GetAllRecords();

            // Assert
            Assert.Equal(GetNumLines(_testCsvData, 1), actualSockets.Count);
        }

        /// <summary>Проверка правильности выполнения выборки</summary>
        [Fact]
        public void Select_isCorrect()
        {
            // Arrange
            const string socketName = "3-640428-7";

            // Act
            var actualSocket = _repositoryMock.Select(socketName).Item2;

            // Assert
            Assert.Equal(socketName, actualSocket.SocketName);
        }

        /// <summary>Проверка правильности выполнения вставки</summary>
        [Fact]
        public void Insert_isCorrect()
        {
            // Arrange
            var insertedSocket = _testSocket;
            var expectedWalEntryLine =
                new WalEntry<CustomSocket>(CsvDbAction.Insert, insertedSocket.GetHashCode(), insertedSocket);

            // Act
            _repositoryMock.Insert(insertedSocket);

            // Assert
            Assert.True(_repositoryMock.GetAllRecords().Contains(insertedSocket),
                "Ожидаемый разъём не найден в репозитории");
            // TODO: Проверить WAL файл
        }

        /// <summary>Проверка правильности выполнения обновления</summary>
        [Fact]
        public void Update_isCorrect()
        {
            // Arrange
            var originalSocket = _repositoryMock.GetAllRecords()[0];
            var hashCode = originalSocket.GetHashCode();
            var updatedSocket = originalSocket;
            updatedSocket.NumberOfContacts++;
            var expectedWalEntryLine =
//                new WalEntry(CsvDbAction.Update, updatedSocket.GetHashCode(), updatedSocket).ToString();

            // Act
            _repositoryMock.Update(hashCode, updatedSocket);

            // Assert
            Assert.True(_repositoryMock.GetAllRecords().Contains(updatedSocket),
                "Обновлённый разъём не найден среди доступных");
            // TODO: Проверить WAL файл
        }

        /// <summary>Проверка правильности выполнения удаления</summary>
        [Fact]
        public void Delete_isCorrect()
        {
            // Arrange
            var deletedScoket = _repositoryMock.GetAllRecords()[0];
            var hashCode = deletedScoket.GetHashCode();
//            var expectedWalEntryLine =
//                new WalEntry(CsvDbAction.Update, deletedScoket.GetHashCode()).ToString();

            // Act
//            _repositoryMock.Delete(hashCode);

            // Assert
            Assert.True(!_repositoryMock.GetAllRecords().Contains(deletedScoket),
                "Удалённый разъём найден среди доступных");
            // TODO: Проверить WAL файл
        }

        /// <inheritdoc />
        /// <summary>Действия по окончании всех тестов</summary>
        public void Dispose()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
        }

        /// <summary>Получить число линий в файле</summary>
        /// <param name="fileName"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
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

    public static partial class FileUtils
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