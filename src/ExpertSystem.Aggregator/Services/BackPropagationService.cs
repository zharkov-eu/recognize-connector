using System;
using ExpertSystem.Aggregator.Models.ANFIS;

namespace ExpertSystem.Aggregator.Services
{
    /// <summary>
    /// Предоставляет методы для обратного распространения ошибки
    /// </summary>
    public class BackPropagationService
    {
        /// <summary>
        /// Возвращает обновление для параметров функции принадлежности
        /// </summary>
        /// <param name="k">Размер шага</param>
        /// <param name="x">Значения выхода сети</param>
        /// <param name="z">Ожидаемые значения</param>
        /// <returns></returns>
        public static InputDegreeParam GetMembershipFuncParamsUpdate(
            int k, double[] x, double[] z)
        {
            var denomirator = 0.0;
            //todo не уверен. что правильно вычисляется проивзодная, это просто шляпа какая-то
            for (var i = 0; i < z.Length; i++)
                denomirator += HalfSquaredEuclidianDistance.CalculatePartialDerivaitveByIndex(z, x, i);

            denomirator = Math.Sqrt(denomirator);
            var trainingSpeed = k / denomirator;

            //var partialDerivative = wtf
            return new InputDegreeParam();
        }
    }

    /// <summary>
    /// Поиск ошибки и её производной с помощью минимизации половины квадрата Евклидова расстояния
    /// </summary>
    public class HalfSquaredEuclidianDistance
    {
        /// <summary>
        /// Вычисляет ошибку на обучающей выборке
        /// </summary>
        /// <param name="z"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double CalculateError(double[] z, double[] x)
        {
            double d = 0;
            for (var i = 0; i < z.Length; i++)
                d += (z[i] - x[i]) * (z[i] - x[i]);
            return 0.5 * d;
        }

        /// <summary>
        /// Вычисляет частную производную ошибки
        /// </summary>
        /// <param name="z"></param>
        /// <param name="x"></param>
        /// <param name="xIndex">Индекс элемента внтури x</param>
        /// <returns></returns>
        public static double CalculatePartialDerivaitveByIndex(double[] z, double[] x, int xIndex)
        {
            return x[xIndex] - z[xIndex];
        }
    }

    /// <summary>
    /// Поиск ошибки и её производной с помощью минимизации логарифмического правдоподобия
    /// </summary>
    public class Loglikelihood
    {
        /// <summary>
        /// Вычисляет ошибку на обучающей выборке
        /// </summary>
        /// <param name="z"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double CalculateError(double[] z, double[] x)
        {
            double d = 0;
            for (var i = 0; i < z.Length; i++)
                d += z[i] * Math.Log(x[i]) + (1 - z[i]) * Math.Log(1 - x[i]);
            return -d;
        }

        /// <summary>
        /// Вычисляет частную производную ошибки
        /// </summary>
        /// <param name="z"></param>
        /// <param name="x"></param>
        /// <param name="xIndex">Индекс элемента внтури x</param>
        public static double CalculatePartialDerivaitveByIndex(double[] z, double[] x, int xIndex)
        {
            return -(z[xIndex] / x[xIndex] - (1 - z[xIndex]) / (1 - x[xIndex]));
        }
    }
}
