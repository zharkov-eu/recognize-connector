using System;
using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;

namespace ExpertSystem.Aggregator.Models.FuzzyLogic
{
    public static class FuzzyCustomSocket
    {
        public static Dictionary<CustomSocketDomain.SocketDomain, int> GetFuzzySocketDomains()
        {
            return new Dictionary<CustomSocketDomain.SocketDomain, int>
            {
                {CustomSocketDomain.SocketDomain.AmperageCircuit, 5},
                {CustomSocketDomain.SocketDomain.NumberOfContacts, 5},
                {CustomSocketDomain.SocketDomain.SizeLength, 3},
                {CustomSocketDomain.SocketDomain.SizeWidth, 3}
            };
        }

        /// <summary>
        ///     NumberOfContacts: 2 - 184 контактов
        ///     SizeLength: 0.0078 - 0.07 м
        ///     SizeWidth: 0.005 - 0.02 м
        ///     Return: 10 - 10000 мА
        /// </summary>
        public static Func<CustomSocket, double> GetAmperageCircuitFormula(
            Dictionary<CustomSocketDomain.SocketDomain, FuzzyRule> rules)
        {
            var nocCluster = rules[CustomSocketDomain.SocketDomain.NumberOfContacts].Cluster + 1; // 1..6
            var slCluster = rules[CustomSocketDomain.SocketDomain.SizeLength].Cluster + 1; // 1..4
            var swCluster = rules[CustomSocketDomain.SocketDomain.SizeWidth].Cluster + 1; // 1..4

            return socket => socket.NumberOfContacts * (Math.Log(nocCluster, 10) + 1) + socket.SizeLength *
                             (Math.Log(slCluster, 2) + 1) * 10 * (socket.SizeWidth * (Math.Log(swCluster, 2) + 1) * 10);
        }
    }
}