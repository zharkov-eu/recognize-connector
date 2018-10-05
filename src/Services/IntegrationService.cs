using System;
using ExpertSystem.Utils;

namespace ExpertSystem.Services
{
    public static class IntegrationService {
        public static T GetSimpsonIntegration<T>(Func<T, T> func, T a, T b) where T : IConvertible
        {
            var aDouble = 0;
            var bDouble = 10;
            var integrationResult = SimpsonIntegrator.Integrate(x => x*x*x, aDouble, bDouble);
            return default(T);
        }
    }    
}