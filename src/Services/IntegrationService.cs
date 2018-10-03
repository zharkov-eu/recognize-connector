using System;

namespace ExpertSystem.Services
{
    public static class IntegrationService {
        public static T GetSimpsonIntegration<T>(Func<T, T> func, T a, T b) where T : IConvertible
        {
            return default(T);
        }
    }    
}