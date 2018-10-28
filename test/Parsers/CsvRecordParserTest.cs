using Xunit;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Parsers;
using ExpertSystem.Server.DAL.Serializers;
using ExpertSystem.Tests.Configuration;

namespace ExpertSystem.Tests.Parsers
{
    /// <summary>Класс для тестирования методов CsvRecordParser</summary>
    public class CsvRecordParserTest
    {
        private readonly CsvRecordParser<CustomSocket> _socketParser;
        private readonly CsvRecordParser<SocketGroup> _socketGroupParser;

        public CsvRecordParserTest()
        {
            _socketParser = new CsvRecordParser<CustomSocket>(new CustomSocketSerializer());
            _socketGroupParser = new CsvRecordParser<SocketGroup>(new SocketGroupSerializer());
        }

        /// <summary></summary>
        [Fact]
        public void SocketParser_GetSockets()
        {
            var sockets = TestData.GetSockets();
            var socket = TestData.GetSocket();

            Assert.True(socket.Equals(sockets[0]),
                "Данные разъема должны десериализироваться правильно");
        }

        /// <summary></summary>
        [Fact]
        public void SocketParser_GetSocketGroups()
        {
            var socketGroups = TestData.GetSocketGroups();
            var socketGroup = TestData.GetSocketGroup();

            Assert.True(socketGroup.Equals(socketGroups[0]),
                "Данные группы разъемов должны десериализироваться правильно");
        }
    }
}