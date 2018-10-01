using System.Collections.Generic;
using ExpertSystem.Models;
using Xunit;

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
			
			_productionProcessor = new ProductionProcessor(rulesGraph, new ProcessorOptions{ Debug = false });
		}
		
		[Fact]
		public void BackwardProcessing_isCorrect()
		{
			// Arrange
			var expectedFactSet = new HashSet<Fact>
			{
				new Fact("NumberOfPositions", 60, typeof(int)),
				new Fact("NumberOfContacts", 120, typeof(int)),
				new Fact("SizeLength", 5.3f, typeof(float)),
				new Fact("MountingStyle", "Through Hole", typeof(string)),
				new Fact("Color", "Natural", typeof(string)),
				new Fact("SizeWidth", 10.0f, typeof(float)),
				new Fact("SizeHeight", 3.2f, typeof(float)),
				new Fact("SizeDiameter", 11.5f, typeof(float)),
				new Fact("HousingMaterial", "Thermoplastic", typeof(string)),
				new Fact("HousingColor", "Natural", typeof(string)),
				new Fact("Material", "Plastic", typeof(string)),
				new Fact("ContactMaterial", "Phosphor Bronze", typeof(string)),
				new Fact("ContactPlating", "Gold", typeof(string)),
				new Fact("PinPitch", 0.00127f, typeof(float)),
				new Fact("Orientation", "Vertical", typeof(string)),
				new Fact("NumberOfRows", 2, typeof(int)),
				new Fact("Gender", "Female", typeof(string)),
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
	            new Fact("NumberOfPositions", 60, typeof(int)),
	            new Fact("NumberOfContacts", 120, typeof(int)),
	            new Fact("MountingStyle", "Through Hole", typeof(string))
	        );

	        const string expectedSocketName = "5145167-4";

	        // Act
	        var socketNameList = _productionProcessor.ForwardProcessing(initialFacts);

	        // Assert
	        Assert.Contains(expectedSocketName, socketNameList);
	    }
	}
}