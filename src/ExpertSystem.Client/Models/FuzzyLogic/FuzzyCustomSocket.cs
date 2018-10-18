using System;
using System.Collections.Generic;
using ExpertSystem.Common.Models;
using static ExpertSystem.Common.Models.CustomSocketDomain;

namespace ExpertSystem.Client.Models.FuzzyLogic
{
    public class FuzzyCustomSocket
    {
        public static Dictionary<SocketDomain, int> GetFuzzySocketDomains()
        {
            return new Dictionary<SocketDomain, int>
            {
                {SocketDomain.AmperageCircuit, 5},
                {SocketDomain.NumberOfContacts, 5},
                {SocketDomain.SizeLength, 3},
                {SocketDomain.SizeWidth, 3}
            };
        }

        /// <summary>
        ///     NumberOfContacts: 2 - 184 контактов
        ///     SizeLength: 0.0078 - 0.07 м
        ///     SizeWidth: 0.005 - 0.02 м
        ///     Return: 10 - 10000 мА
        /// </summary>
        /// <param name="clusterCount">Число кластеров</param>
        /// <param name="values">Значения кластеров</param>
        /// <param name="m">Чёткость алгоритма</param>
        /// <param name="eps">Точность алгоритма</param>
        public static Func<CustomSocket, double> GetAmperageCircuitFormula(Dictionary<SocketDomain, FuzzyRule> rules)
        {
            var nocCluster = rules[SocketDomain.NumberOfContacts].Cluster + 1; // 1..6
            var slCluster = rules[SocketDomain.SizeLength].Cluster + 1; // 1..4
            var swCluster = rules[SocketDomain.SizeWidth].Cluster + 1; // 1..4

            return socket => socket.NumberOfContacts * (Math.Log(nocCluster, 10) + 1) + socket.SizeLength *
                             (Math.Log(slCluster, 2) + 1) * 10 * (socket.SizeWidth * (Math.Log(swCluster, 2) + 1) * 10);
        }
    }
}
