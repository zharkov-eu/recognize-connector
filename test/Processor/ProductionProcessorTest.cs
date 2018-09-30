using System.Collections.Generic;
using System.IO;
using System.Text;
using ExpertSystem.Models;
using Xunit;

namespace ExpertSystem.Processor.ProductionProcessor
{
	public class ProductionProcessorTest
	{
		private readonly CustomSocket _socket;
		private readonly SocketFieldsProcessor _processor;
		private readonly ProductionProcessor _productionProcessor;

		public ProductionProcessorTest()
		{
			_socket = new CustomSocket();
			_processor = new SocketFieldsProcessor();
			
			_socket.Color = "Natural";
			_socket.SocketName = "5145167-4";
			_socket.ContactMaterial = "Phosphor Bronze";
			_socket.ContactPlating = "Gold";
			_socket.Gender = "Female";
			_socket.HousingColor = "Natural";
			_socket.HousingMaterial = "Thermoplastic";
			_socket.MountingStyle = "Through Hole";
			_socket.NumberOfContacts = 120;
			_socket.NumberOfPositions = 60;
			_socket.NumberOfRows = 2;
			_socket.Orientation = "Vertical";
			_socket.PinPitch = 0.00127f;
			_socket.Material = "Plastic";
			_socket.SizeDiameter = 11.5f;
			_socket.SizeLength = 5.3f;
			_socket.SizeHeight = 3.2f;
			_socket.SizeWidth = 10.0f;
			
			var csvSnapshot = "mpn;color;contact_material;contact_plating;gender;housing_color;housing_material;mounting_style;number_of_contacts;number_of_positions;number_of_rows;orientation;pin_pitch;material;size_diameter;size_length;size_height;size_width\n";
			csvSnapshot += "5145167-4;Natural;Phosphor Bronze;Gold;Female;Natural;Thermoplastic;Through Hole;120;60;2;Vertical;0.00127;Plastic;11.5;5.3;3.2;10.0\n";
			csvSnapshot += "1-329631-2;Silver;;;;;;;;;;;;;;;;\n";
            csvSnapshot += "1-5145154-2;White;Phosphor Bronze;Gold;Female;Natural;Thermoplastic;Solder;120;60;2;Vertical;0.00127;Thermoplastic;;;;\n";
            csvSnapshot += "R3MZ;;;Nickel;Male;;;Solder;3;3;;;;;0.024384;0.06985;;\n";
            csvSnapshot += "A3MB;Black;;Silver;Male;Black;;Solder;3;3;;;;;0.018796;0.070612;;\n";
            csvSnapshot += "211882-1;;;;Male;Gray;Thermoplastic;;4;4;;;;Thermoplastic;;;;\n";
            csvSnapshot += "5145154-4;White;Phosphor Bronze;Gold;Female;Natural;Thermoplastic;Solder;120;60;2;Straight;0.00127;Thermoplastic;;;;\n";
            csvSnapshot += "1-208657-1;Black;Phosphor Bronze;Gold;Female;Black;Nylon;Flange;8;8;;;;;;;;\n";
            csvSnapshot += "CR2-M;Natural;;;;;;;;;;;;Nylon;;0.0084;0.005;0.0051\n";
            csvSnapshot += "2106710-1;Natural;Phosphor Bronze;Tin;Female;Natural;;Screw;;;;;;;;;;\n";
            csvSnapshot += "MS3476W14-19P;;;Gold;Male;;;;19;;;;;;;;;\n";
            csvSnapshot += "C3M;Silver;;Nickel;Male;;;Panel;3;3;;;;;0.0206502;0.0256032;;\n";
            csvSnapshot += "8-215570-0;Red;Phosphor Bronze;Tin;Male;Red;Polyester;Through Hole;10;10;;Vertical;0.00127;Polyester;;;;\n";
            csvSnapshot += "D5M;Silver;Gold;Gold;Male;;;Panel;5;5;;;;;0.018923;0.0206502;;\n";
            csvSnapshot += "8-215570-2;Red;Phosphor Bronze;Tin;Male;Red;Polyester;Through Hole;12;12;;Vertical;0.00127;Polyester;;;;\n";
            csvSnapshot += "MS3476L22-55P;;;Gold;Male;;;;55;;;;;;;;;\n";
            csvSnapshot += "EE-1003A;;;;;;;Bracket;;;;;;;;;;\n";
            csvSnapshot += "5145154-1;Natural;Phosphor Bronze;Gold Flash;Female;Natural;Thermoplastic;Through Hole;120;;;Vertical;;;;;;\n";
            csvSnapshot += "84952-8;Natural;Phosphor Bronze;Tin;Female;White;;Surface Mount;8;;;Right Angle;0.001;Thermoplastic;;;;\n";
            csvSnapshot += "1-206455-2;Black;;Gold;Male;Black;Thermoplastic;Flange;63;63;;;;;;;;\n";
            csvSnapshot += "20ESRM-3;;;;Male;;;Panel;;;;;;;;;;\n";
            csvSnapshot += "CR4H-M0;Natural;;;;;;;;;;;;Nylon;;0.0146;0.0076;0.0091\n";
            csvSnapshot += "TA5FLX;;Copper Alloy;Tin;Male;;;Solder;5;;;;;;;;;\n";
            csvSnapshot += "6331G1;Red;Copper;Silver;Male;Red;;Solder;2;;;;;;;;;\n";
			
			List<CustomSocket> sockets;
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvSnapshot)))
				sockets = _processor.GetSockets(stream);
			
			var fieldValues = _processor.GetFieldsWithPossibleValues(sockets);
			
			var rulesGenerator = new RulesGenerator();
			var rulesGraph = rulesGenerator.GenerateRules(sockets, fieldValues);
			
			_productionProcessor = new ProductionProcessor(rulesGraph, new ProcessorOptions{ Debug = true });
		}
		
		[Fact]
		public void BackwardProcessing_isCorrect()
		{
			// Arrange
			HashSet<Fact> expectedFactSet = new HashSet<Fact>()
			{
				new Fact("NumberOfPositions", "60"),
				new Fact("NumberOfContacts", "120"),
				new Fact("SizeLength", "5.3"),
				new Fact("MountingStyle", "Through Hole"),
				new Fact("Color", "Natural"),
				new Fact("SizeWidth", "10"),
				new Fact("SizeHeight", "3.2"),
				new Fact("SizeDiameter", "11.5"),
				new Fact("HousingMaterial", "Thermoplastic"),
				new Fact("HousingColor", "Natural"),
				new Fact("Material", "Plastic"),
				new Fact("ContactMaterial", "Phosphor Bronze"),
				new Fact("ContactPlating", "Gold"),
				new Fact("PinPitch", "0.00127"),
				new Fact("Orientation", "Vertical"),
				new Fact("NumberOfRows", "2"),
				new Fact("Gender", "Female"),
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
			FactSet initialFacts = new FactSet(
				new Fact("NumberOfPositions", "60"),
				new Fact("NumberOfContacts", "120"),
				new Fact("MountingStyle", "Through Hole")
			);
			
			const string expectedSocketName = "5145167-4";
			
			// Act
			var socketNameList = _productionProcessor.ForwardProcessing(initialFacts);
			
			// Assert
			Assert.Contains(expectedSocketName, socketNameList);
		}
		
	}
}
