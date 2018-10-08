using System;
using System.Linq;
using System.Collections.Generic;
using ExpertSystem.Models;
using ExpertSystem.Utils;

namespace ExpertSystem.Services
{
    public class FuzzyValue
    {
        public double Value { get; set; }
        public Dictionary<int, double> ClusterDegree { get; set; }

        public FuzzyValue(double value, Dictionary<int, double> clusterDegree)
        {
            Value = value;
            ClusterDegree = clusterDegree;
        }

		public KeyValuePair<int, double> GetMostProbableCluster()
		{
			return ClusterDegree.GroupBy(kv => kv.Value).OrderByDescending(g => g.Key).First().First();
		}
    }
    
    public static class ClusteringService
    {
        /// <summary>
		/// Алгоритм нечёткой кластеризации 
		/// </summary>
		/// <param name="clusterCount">Число кластеров</param>
		/// <param name="values">Значения кластеров</param>
		/// <param name="m">Чёткость алгоритма</param>
		/// <param name="eps">Точность алгоритма</param>
        public static IEnumerable<FuzzyValue> CMeans(int clusterCount, List<double> values, double m = 2, double eps = 0.001)
        {
            if (!values.Any())
                throw new Exception("Values is empty");

            // Создаём матрицу принадлежности
            double[][] u = Enumerable.Range(0, values.Count).Select(x => {
				double[] c = new double[clusterCount];
				for (int i = 0; i < c.Length; i++)
				{
					c[i] = RandUtil.RandDoubleRange(0, 1 - c.Sum());
					if (i == c.Length - 1) c[i] = 1 - c.Sum();
				}
				return c;	
			}).ToArray();

            double _j = -1; // Текущее значение целевой функции
			List<double> centers; // Список центров кластеров

            while (true)
			{
				// Генерация новых центров кластеров
				centers = GetCenters(clusterCount, values, m, u);
				// Получение значения целевой функции
				double newJ = ObjectiveFunc(values, m, u, centers);
				if (_j != -1 && Math.Abs(newJ - _j) < eps) break;
				_j = newJ;

				// Обновление матрицы принадлежности
				for (int i = 0; i < u.Length; i++)
				{
					for (int j = 0; j < u[i].Length; j++)
					{
						double denominator = Enumerable.Range(0, clusterCount).Select(x =>
							Math.Pow(
								Math.Sqrt(Dist(values[i], centers[j])) / Math.Sqrt(Dist(values[i], centers[x])),
								2/(m-1)
							)
						).Sum();

						u[i][j] = 1 / denominator;
						if (double.IsNaN(u[i][j])) u[i][j] = 1;
					}
				}
			}

            var fuzzyValues = new List<FuzzyValue>();
            for (int j = 0; j < values.Count; j++)
            {
                var clusterDegree = new Dictionary<int, double>();
                for (int k = 0; k < u[j].Count(); k++)
                    clusterDegree.Add(k, u[j][k]);
                fuzzyValues.Add(new FuzzyValue(values[j], clusterDegree));
            }

            return fuzzyValues;
        }

        /// <summary>
		/// Генерация центров кластеров
		/// </summary>
		/// <param name="clusterCount">Число кластеров</param>
		/// <param name="values">Значения кластеров</param>
		/// <param name="m">Чёткость алгоритма</param>
		/// <param name="u">Матрица принадлежности</param>
		/// <returns>Список центров кластеров</returns>
		private static List<double> GetCenters(int clusterCount, List<double> values, double m, double[][] u)
		{
			return Enumerable.Range(0, clusterCount).Select(i =>
                Enumerable.Range(0, values.Count).Select(j =>
                    Math.Pow(u[j][i], m) * values[j]
                ).Sum()
                /
                Enumerable.Range(0, values.Count).Select(j =>
                    Math.Pow(u[j][i], m)
                ).Sum()
			).ToList();
		}

        /// <summary>
		/// Целевая функция
		/// </summary>
		/// <param name="values">Значения</param>
		/// <param name="m">Чёткость алгоритма</param>
		/// <param name="u">Матрица принадлежности</param>
		/// <param name="centers">Центроиды</param>
		/// <returns>Значение целевой функции</returns>
		private static double ObjectiveFunc(List<double> values, double m, double[][] u, List<double> centers)
		{
			return centers.Select((x, i) =>
				values.Select((y, j) =>
					Math.Pow(u[j][i], m) * Dist(y,x)
				).Sum()
			).Sum();
		}

		/// <summary>
		/// Евклидово расстояния
		/// </summary>
		/// <param name="a">Точка А</param>
		/// <param name="b">Точка Б</param>
		/// <returns></returns>
		private static double Dist(double a, double b)
		{
			return Math.Pow(a - b, 2);	
		}
    }    
}