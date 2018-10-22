using System;
using System.IO;
using System.Linq;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.DAL.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace ExpertSystem.Tests.DAL.Repositories
{
	/// <summary>Класс тестирования методов CsvRepository</summary>
	// TODO: Имплементировать незаконченные тесты
	public class CsvRepositoryTest
	{
		// Для вывода данных теста в консоль
		private readonly ITestOutputHelper _output;

		private readonly string _testCsvFileName;
		private readonly string _testWalFileName;
		// Мок репозитория
		private readonly CsvRepository _repositoryMock;
		// Тестовые данные
		private readonly CustomSocket _testSocket;

		public CsvRepositoryTest(ITestOutputHelper output)
		{
			_output = output;

			// Заполняем тестовые данные
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

			// Создаём и заполняем тестовые файлы
			var appTestPath =
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ExpertSystem");
			_testCsvFileName = Path.Combine(appTestPath, "csvTest.csv");
			File.Create(_testCsvFileName);
			// TODO: Заполнить CSV файл
			_testWalFileName = Path.Combine(appTestPath, "walTest.txt");
			_repositoryMock = new CsvRepository(_testCsvFileName, _testWalFileName);
		}

		/// <summary>Проверка верности синхронизации</summary>
		[Fact]
		public void Sync_isCorrect()
		{
			// Arrange

			// Act
			_repositoryMock.Sync();

			// Assert
			// Число записей в csv равно число записей в CsvRepository._sockets
			Assert.Equal(new FileInfo(_testCsvFileName).Length, _repositoryMock.GetSockets().Count);
			// TODO: проверить что WAL файл существует и пустой
		}

		/// <summary>Проверка верности получения списка разъёмов</summary>
		[Fact]
		public void GetSockets_isCorrect()
		{
			// Arrange

			// Act
			_repositoryMock.Sync();
			var actualSockets = _repositoryMock.GetSockets();

			// Assert
			// Число записей в csv равно число записей в CsvRepository._sockets
			Assert.Equal(new FileInfo(_testCsvFileName).Length, actualSockets.Count);
		}

		/// <summary>Проверка правильности выполнения выборки</summary>
		[Fact]
		public void Select_isCorrect()
		{
			// Arrange
			var socketName = _testSocket.SocketName;
			var expectedSocket = _testSocket;

			// Act
			var actualSocket = _repositoryMock.Select(socketName).Item2;

			// Assert
			Assert.Equal(expectedSocket, actualSocket);
		}

		/// <summary>Проверка правильности выполнения вставки</summary>
		[Fact]
		public void Insert_isCorrect()
		{
			// Arrange
			var inputSocket = _testSocket;
			var expectedWalEntryLine = 
				new CsvEntity(CsvDbAction.Insert, inputSocket.GetHashCode(), inputSocket).ToString();
			// TODO: Set to be right value
			var testWalFile = Path.Combine("..", "..");

			// Act
			_repositoryMock.Insert(inputSocket);

			// Assert
			Assert.True(File.ReadAllText(testWalFile).Contains(expectedWalEntryLine),
				"Ожидаемая строка отсутсвует в WAL файле");
			// TODO: Проверить что разъём есть в репозитории
//			Assert.Equal(_repositoryMock.Select(inputSocket.SocketName).Item1, inputSocket);
		}

		/// <summary>Проверка правильности выполнения обновления</summary>
		[Fact]
		public void Update_isCorrect()
		{
			// Arrange
			
			// Act
			
			// Assert
			
		}

		/// <summary>Проверка правильности выполнения удаления</summary>
		[Fact]
		public void Delete_isCorrect()
		{
			// Arrange
			var socketName = _testSocket.SocketName;
			const int hashCode = 1;
			_repositoryMock.Sync();

			// Act
			_repositoryMock.Delete(hashCode);

			// Assert
			Assert.False(_repositoryMock.GetSockets().Select(socket => socket.SocketName == socketName).First());
		}
	}
}