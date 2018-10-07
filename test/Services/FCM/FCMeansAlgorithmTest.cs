using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ExpertSystem.Services.FCM
{
	public class FCMeansAlgorithmTest
	{
		private readonly ITestOutputHelper _output;
		
		private readonly List<double[]> _testData;
		
		public FCMeansAlgorithmTest(ITestOutputHelper output)
		{
			// Для вывода значений xUnit в консоль 
			_output = output;
			
			// Генерируем тестовые данные
			Random rand = new Random(DateTime.Now.Millisecond);
			_testData = Enumerable.Range(0, 10).Select(x => new double[] {rand.NextDouble()}).ToList();
		}

		[Fact]
		public void CMeans_isCorrect()
		{
			// Прогоняем
			List<FuzzyCluster> result = FCMeansAlgorithm.CMeans(10, _testData);
			// TODO: Интерпретировать и проврить результат
		}
	}
}