using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using Xunit;

namespace ExpertSystem.Tests.Models
{
	/// <summary>Класс для тестирования методов SocketGroupExtension</summary>
	public class SocketGroupExtensionTest
	{

		private readonly string _line;
		private readonly string[] _parts;
		private readonly SocketGroup _group;
		
		public SocketGroupExtensionTest()
		{
			_line = "Audio;5145167-4;XF2L-0425-1A;";
			_parts = _line.Split(SocketGroupExtension.Delimiter);
			
			_group = new SocketGroup
			{
				GroupName = "Audio",
				SocketNames = {"5145167-4", "XF2L-0425-1A"}
			};
		}

		/// <summary>Проверка десериалзициции CSV частей в SocketGroup</summary>
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

		/// <summary>Проверка десериалзициции CSV строки в SocketGroup</summary>
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

		/// <summary>Проверка десериалзициции CSV частей в SocketGroup</summary>
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