using System;
using System.Collections.Generic;
using System.Linq;
using ExpertSystem.Utils;

namespace ExpertSystem.Services.FCM
{
	/// <summary>
	/// Нечёткий кластер
	/// </summary>
	public class FuzzyCluster
	{
		// Номер кластера
		public int Id { get; set; }
		// Список пар значение - мера принадлежности
		public List<double[]> Values { get; set; } = new List<double[]>();
	}
	
	/// <summary>
	/// Нечёткий алгоритм C-средних 
	/// </summary>
	public static class FCMeansAlgorithm
	{
		public static List<FuzzyCluster> CMeans(int numClusters, List<double[]> Values) => CMeans(numClusters, Values, 2, 0.001);
		public static List<FuzzyCluster> CMeans(int numClusters, List<double[]> Values, double m) => CMeans(numClusters, Values, m, 0.001);
		/// <summary>
		/// Алгоритм нечёткой кластеризации 
		/// </summary>
		/// <param name="numClusters">Число кластеров???</param>
		/// <param name="values">Значения кластеров</param>
		/// <param name="m">Чёткость алгоритма</param>
		/// <param name="eps">Точность алгоритма</param>
		/// <returns></returns>
		public static List<FuzzyCluster> CMeans(int numClusters, List<double[]> values, double m, double eps)
		{
			// Создаём матрицу принадлежности
			double[][] u = Enumerable.Range(0, values.Count).Select(x => {
				double[] c = new double[numClusters];
				for (int i = 0; i < c.Length; i++)
				{
					c[i] = RandUtil.RandDoubleRange(0, 1 - c.Sum());
					if (i == c.Length - 1) c[i] = 1 - c.Sum();
				}
				return c;	
			}).ToArray(); 
			
			double _j = -1; // Текущее значение целевой функции
			List<double[]> centers; // Список центров кластеров
			// Основная часть алгоритма
			while (true)
			{
				// Генерация новых центров кластеров
				centers = GetCenters(numClusters, values, m, u);
				// Получение значения целевой функции
				double newJ = ObjectiveFunc(values, m, u, centers);
				if (_j != -1 && Math.Abs(newJ - _j) < eps) break;
				_j = newJ;

				// Обновление матрицы принадлежности
				for (int i = 0; i < u.Length; i++)
				{
					for (int j = 0; j < u[i].Length; j++)
					{
						double denominator = Enumerable.Range(0, numClusters).Select(x =>
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

			// Генерация результата
			List<FuzzyCluster> result = Enumerable.Range(0, numClusters).Select((x, i) => 
				new FuzzyCluster() {Id = i }
			).ToList();
			for (int i = 0; i < values.Count; i++)
			{
				int index = Array.IndexOf(u[i], u[i].Max());
				result[index].Values.Add(values[i]);
			}
			return result;
		}

		/// <summary>
		/// Генерация центров кластеров
		/// </summary>
		/// <param name="classCount">Число кластеров???</param>
		/// <param name="values">Значения кластеров</param>
		/// <param name="m">Чёткость алгоритма</param>
		/// <param name="u">Матрица принадлежности</param>
		/// <returns>Список центров кластеров</returns>
		private static List<double[]> GetCenters(int classCount, List<double[]> values, double m, double[][] u)
		{
			return Enumerable.Range(0, classCount).Select(i => //List<double[]>
				Enumerable.Range(0, values.First().Length).Select(x =>//double[]
					Enumerable.Range(0, values.Count).Select(j =>
						Math.Pow(u[j][i], m) * values[j][x]
					).Sum()
					/
					Enumerable.Range(0, values.Count).Select(j =>
						Math.Pow(u[j][i], m)
					).Sum()
				).ToArray()
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
		private static double ObjectiveFunc(List<double[]> values, double m, double[][] u, List<double[]> centers)
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
		/// <param name="valueA">Точка А</param>
		/// <param name="valueB">Точка Б</param>
		/// <returns></returns>
		private static double Dist(double[] valueA, double[] valueB)
		{
			return valueA.Select((x, i) => 
				Math.Pow(x - valueB[i], 2)	
			).Sum();
		}
	}
}
