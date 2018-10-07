using System;

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
}