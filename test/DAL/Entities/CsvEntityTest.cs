using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Entities;
using Xunit;
using Xunit.Abstractions;

namespace ExpertSystem.Tests.DAL.Entities
{
	public class CsvEntityTest
	{
		// Для вывода данных теста в консоль
		private readonly ITestOutputHelper _output;

		// Тестовые данные
		private readonly CustomSocket _testSocket;
		private readonly CsvEntity _testEntryInsert;
		private readonly CsvEntity _testEntryUpdate;
		private readonly CsvEntity _testEntryDelete;

		private readonly string _testCsvLine;

		public CsvEntityTest(ITestOutputHelper output)
		{
			_output = output;

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
			_testEntryInsert = new CsvEntity(CsvDbAction.Insert, _testSocket.GetHashCode(), _testSocket);
			_testEntryUpdate = new CsvEntity(CsvDbAction.Update, _testSocket.GetHashCode(), _testSocket);
			_testEntryDelete = new CsvEntity(CsvDbAction.Delete, _testSocket.GetHashCode());
			_testCsvLine = 
				"5747871-8;Male;Brass;Tin;Black;Black;Thermoplastic;Through Hole;9;9;2;Vertical;0.002743;;;0.03081;0.008128;0.012548;";
		}

		/// <summary>Проверка правильности преобразования WAL записи вставки в строку</summary>
		[Fact]
		public void ToString_Insert_isCorrect()
		{
			// Arrange
			var expectedString = $"Insert:::{_testSocket.GetHashCode()}:::{_testCsvLine}";

			// Act
			var actualString = _testEntryInsert.ToString();

			// Assert
			Assert.Equal(expectedString, actualString);
		}

		/// <summary>Проверка правильности преобразования WAL записи обновления в строку</summary>
		[Fact]
		public void ToString_Update_isCorrect()
		{
			// Arrange
			var expectedString = $"Update:::{_testSocket.GetHashCode()}:::{_testCsvLine}";

			// Act
			var actualString = _testEntryUpdate.ToString();

			// Assert
			Assert.Equal(expectedString, actualString);
		}

		/// <summary>Проверка правильности преобразования WAL записи удаления в строку</summary>
		[Fact]
		public void ToString_Delete_isCorrect()
		{
			// Arrange
			var expectedString = $"Delete:::{_testSocket.GetHashCode()}:::";

			// Act
			var actualString = _testEntryDelete.ToString();

			// Assert
			Assert.Equal(expectedString, actualString);
		}
	}
}