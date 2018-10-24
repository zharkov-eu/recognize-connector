using Xunit;
using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Client.Models;
using ExpertSystem.Client.Processors;
using ExpertSystem.Client.RulesGenerators;
using ExpertSystem.Tests.Parsers;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Tests.Processors
{
    public class ProductionProcessorTest
    {
        private readonly CustomSocket _socket;
        private readonly ProductionProcessor _productionProcessor;

        public ProductionProcessorTest()
        {
            var socketParser = new CustomParserTest();
            var sockets = socketParser.GetSockets();

            var rulesGenerator = new ProductionRulesGenerator();
            var rulesGraph = rulesGenerator.GenerateRules(sockets);

            _socket = socketParser.TestSocket();
            _productionProcessor = new ProductionProcessor(rulesGraph, new ProcessorOptions { Debug = false });
        }

        [Fact]
        public void BackwardProcessing_isCorrect()
        {
            // Arrange
            var expectedFactSet = new HashSet<Fact>
            {
                new Fact(SocketDomain.NumberOfPositions, 60),
                new Fact(SocketDomain.NumberOfContacts, 120),
                new Fact(SocketDomain.SizeLength, 0.0145f),
                new Fact(SocketDomain.MountingStyle, "Through Hole"),
                new Fact(SocketDomain.Color, "Natural"),
                new Fact(SocketDomain.SizeWidth, 0.0034f),
                new Fact(SocketDomain.SizeHeight, 0.00415f),
                new Fact(SocketDomain.SizeDiameter, 0.015f),
                new Fact(SocketDomain.HousingMaterial, "Thermoplastic"),
                new Fact(SocketDomain.HousingColor, "Natural"),
                new Fact(SocketDomain.Material, "Plastic"),
                new Fact(SocketDomain.ContactMaterial, "Phosphor Bronze"),
                new Fact(SocketDomain.ContactPlating, "Gold"),
                new Fact(SocketDomain.PinPitch, 0.00127f),
                new Fact(SocketDomain.Orientation, "Vertical"),
                new Fact(SocketDomain.NumberOfRows, 2),
                new Fact(SocketDomain.Gender, "Female")
            };

            const string socketName = "5145167-4";

            // Act
            var factSet = _productionProcessor.BackProcessing(socketName);

            // Assert
            Assert.Equal(expectedFactSet, factSet.Facts);
        }

        [Fact]
        public void ForwardProcessing_isCorrect()
        {
            // Arrange
            var initialFacts = new FactSet(
                new Fact(SocketDomain.NumberOfPositions, 60),
                new Fact(SocketDomain.NumberOfContacts, 120),
                new Fact(SocketDomain.MountingStyle, "Through Hole")
            );

            const string expectedSocketName = "5145167-4";

            // Act
            var socketNameList = _productionProcessor.ForwardProcessing(initialFacts);

            // Assert
            Assert.Contains(expectedSocketName, socketNameList);
        }
    }
}