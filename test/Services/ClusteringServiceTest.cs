using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using ExpertSystem.Aggregator.Services;

namespace ExpertSystem.Tests.Services
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
            var plainClusters = new List<double> {1, 1, 1, 5, 5, 5, 9, 9, 9};
            const int plainClustersExpectedCount = 3;
            const int elements = 3;

            var result = ClusteringService.CMeans(plainClustersExpectedCount, plainClusters).ToList();

            for (var k = 0; k < plainClustersExpectedCount; k++)
            {
                var clusterDegree = result[k * elements].GetMostProbableCluster();
                for (var j = k * elements; j < elements; j++)
                    Assert.Equal(clusterDegree.Key, result[j].GetMostProbableCluster().Key);
            }
        }

        [Fact]
        public void CMeans_isComplexCorrect()
        {
            var complexClusters = new List<double>
            {
                2,
                2,
                3,
                3,
                3,
                3,
                3,
                3,
                3,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                4,
                5,
                5,
                5,
                5,
                5,
                5,
                6,
                6,
                7,
                7,
                7,
                8,
                8,
                8,
                8,
                8,
                8,
                8,
                8,
                8,
                10,
                10,
                10,
                10,
                10,
                10,
                12,
                12,
                12,
                14,
                15,
                15,
                16,
                16,
                19,
                19,
                19,
                19,
                20,
                22,
                22,
                26,
                28,
                28,
                29,
                30,
                37,
                37,
                38,
                41,
                55,
                55,
                55,
                55,
                63,
                64,
                70,
                120,
                120,
                120,
                120,
                120,
                120,
                184
            };
            const int complexClustersExpectedCount = 3;
            var result = ClusteringService.CMeans(complexClustersExpectedCount, complexClusters).ToList();
        }

        [Fact]
        public void CMeans_isSeedCapable()
        {
            var values = new List<double>
            {
                1,
                1,
                2,
                3,
                3,
                4,
                5,
                5
            };

            var fuzzyValues = ClusteringService.CMeans(3, values, randSeed: 0).ToList();
        }
    }
}