using System;
using System.Collections.Generic;

namespace ExpertSystem.Services
{
    public class FuzzyValue<T> where T : IConvertible
    {
        Dictionary<int, T> ClusterDegree;
    }
    public static class ClusterizationService
    {
        public static IEnumerable<FuzzyValue<T>> CMeansClusterization<T>(IEnumerable<T> values) where T : IConvertible
        {
            return new List<FuzzyValue<T>>();
        }
    }    
}