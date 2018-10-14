using System.Collections.Generic;
using ExpertSystem.Models;
using Xunit;
using static ExpertSystem.Models.CustomSocketDomain;

namespace ExpertSystem.Processor.ProductionProcessor
{
    public class ProductionProcessorTest
    {
        private readonly CustomSocket _socket;
        private readonly ProductionProcessor _productionProcessor;

        public ProductionProcessorTest()
        {
            _socket = new CustomSocket
            {
                Color = "Natural",
                SocketName = "5145167-4",
                ContactMaterial = "Phosphor Bronze",
                ContactPlating = "Gold",
                Gender = "Female",
                HousingColor = "Natural",
                HousingMaterial = "Thermoplastic",
                MountingStyle = "Through Hole",
                NumberOfContacts = 120,
                NumberOfPositions = 60,
                NumberOfRows = 2,
                Orientation = "Vertical",
                PinPitch = 0.00127f,
                Material = "Plastic",
                SizeDiameter = 11.5f,
                SizeLength = 5.3f,
                SizeHeight = 3.2f,
                SizeWidth = 10.0f
            };

            var socketFieldsProcessor = new SocketFieldsProcessorTest();
            var sockets = socketFieldsProcessor.GetSockets();
            var fieldValues = socketFieldsProcessor.GetFieldsWithPossibleValues(sockets);

            var rulesGenerator = new RulesGenerator();
            var rulesGraph = rulesGenerator.GenerateRules(sockets, fieldValues);

            _productionProcessor = new ProductionProcessor(rulesGraph, new ProcessorOptions {Debug = false});
        }

        [Fact]
        public void BackwardProcessing_isCorrect()
        {
            // Arrange
            var expectedFactSet = new HashSet<Fact>
            {
                new Fact(SocketDomain.NumberOfPositions, 60),
                new Fact(SocketDomain.NumberOfContacts, 120),
                new Fact(SocketDomain.SizeLength, 5.3f),
                new Fact(SocketDomain.MountingStyle, "Through Hole"),
                new Fact(SocketDomain.Color, "Natural"),
                new Fact(SocketDomain.SizeWidth, 10.0f),
                new Fact(SocketDomain.SizeHeight, 3.2f),
                new Fact(SocketDomain.SizeDiameter, 11.5f),
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