using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ExpertSystem.Services
{
	public class ClusteringServiceTest
	{
		private readonly ITestOutputHelper _output;
		
		public ClusteringServiceTest(ITestOutputHelper output)
		{
			// Для вывода значений xUnit в консоль 
			_output = output;
		}

		[Fact]
		public void CMeans_isPlainCorrect()
		{
			List<double> plainClusters = new List<double> { 1, 1, 1, 5, 5, 5, 9, 9, 9 };
			const int plainClustersExpectedCount = 3;	
			const int elements = 3;

			List<FuzzyValue> result = ClusterizationService.CMeans(plainClustersExpectedCount, plainClusters).ToList();

			for (int k = 0; k < plainClustersExpectedCount; k++)
			{
				var clusterDegree = result[k * elements].GetMostProbableCluster();
				var threeElements = plainClusters.GetRange(k * elements, elements);
				for (int j = k * elements; j < elements; j++)
					Assert.Equal(clusterDegree.Cluster, result[j].GetMostProbableCluster().Cluster);
			}
		}

		[Fact]
		public void CMeans_isComplexCorrect()
		{
			List<double> complexClusters = new List<double> { 2, 2, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 6, 6, 7, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 10, 10, 10, 10, 10, 10, 12, 12, 12, 14, 15, 15, 16, 16, 19, 19, 19, 19, 20, 22, 22, 26, 28, 28, 29, 30, 37, 37, 38, 41, 55, 55, 55, 55, 63, 64, 70, 120, 120, 120, 120, 120, 120, 184 };
			const int complexClustersExpectedCount = 3;	
			List<FuzzyValue> result = ClusterizationService.CMeans(complexClustersExpectedCount, complexClusters).ToList();
		}
	}
}