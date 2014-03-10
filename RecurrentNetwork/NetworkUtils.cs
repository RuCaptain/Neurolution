using System;

namespace RecurrentNetworkLibrary
{
    public static class NetworkUtils
    {
        private static readonly Random Random = new Random();

        public static float RandomRange(float minValue, float maxValue)
        {
            return minValue + (float)Random.NextDouble()*(maxValue - minValue);
        }

        public static float RandomSpread(float value, float spread)
        {
            return RandomRange(value*(1 - spread), value/(1 - spread));
        }

        public static float Range(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
