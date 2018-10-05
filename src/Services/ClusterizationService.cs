using System;
using System.Collections.Generic;

namespace ExpertSystem.Services
{
    public class FuzzyValue<T> where T : IConvertible
    {
        public Dictionary<int, T> ClusterDegree { get; set; }
        public T Value { get; set; }
    }
    public static class ClusterizationService
    {
        public static IEnumerable<FuzzyValue<T>> CMeansClusterization<T>(IEnumerable<T> values) where T : IConvertible
        {
            return new List<FuzzyValue<T>>();
        }
    }    
}