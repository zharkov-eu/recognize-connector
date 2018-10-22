using System;
using System.Security.Cryptography;

namespace ExpertSystem.Common.Utils
{
    public static class RandUtil
    {
        private static Random _rand = new Random(DateTime.Now.Millisecond);

        public static void SetSeed(int seed)
        {
            _rand = new Random(seed);
        }

        public static double RandDoubleRange(double from, double to)
        {
            return _rand.NextDouble() * (to - from) + from;
        }
    }

    public class CryptoRandom
    {
        public CryptoRandom()
        {
            using (var p = new RNGCryptoServiceProvider())
            {
                var r = new Random(p.GetHashCode());
                RandomValue = r.NextDouble();
            }
        }

        public double RandomValue { get; set; }
    }
}