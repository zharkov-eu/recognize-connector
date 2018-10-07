using System;
using System.Collections.Generic;

namespace ExpertSystem.Services
{
    public class FuzzyValue<T> where T : IConvertible
    {
        public Dictionary<int, T> ClusterDegree { get; set; }
        public T Value { get; set; }
    }
    
    // TODO: Переименовать в ClustringService
    public static class ClusterizationService
    {
        public static IEnumerable<FuzzyValue<T>> CMeansClusterization<T>(IEnumerable<T> values) where T : IConvertible
        {
            // TODO: Подключить FCMeansAlgorithm.CMeans()
            return new List<FuzzyValue<T>>();
        }
    }    
}