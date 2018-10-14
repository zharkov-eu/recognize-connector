using System;
using System.Security.Cryptography;

namespace ExpertSystem.Utils
{
	public static class RandUtil
	{
		private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

		public static double RandDoubleRange(double from, double to)
		{
			return Rand.NextDouble() * (to - from) + from;
		}
	}

    public class CryptoRandom
    {
        public double RandomValue { get; set; }

        public CryptoRandom()
        {
            using (var p = new RNGCryptoServiceProvider())
            {
                var r = new Random(p.GetHashCode());
                RandomValue = r.NextDouble();
            }
        }
    }
}