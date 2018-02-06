using System;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="Random"/>. The code is taken from here:
    /// https://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Generates a random integer that can be any number.
        /// </summary>
        /// <param name="random">The random.</param>
        /// <returns>A random integer.</returns>
        public static int NextInt32(this Random random)
        {
            var firstBits = random.Next(0, 1 << 4) << 28;
            var lastBits = random.Next(0, 1 << 28);

            return firstBits | lastBits;
        }

        /// <summary>
        /// Generates a random decimal number that is at least 0 and less than 1.
        /// </summary>
        /// <param name="random">The random.</param>
        /// <returns>A random decimal number from the interval [0,1).</returns>
        public static decimal NextDecimal(this Random random)
        {
            var sample = 1m;

            //After ~200 million tries this never took more than one attempt but it is possible 
            // to generate combinations of a, b, and c with the approach below resulting in a sample >= 1.
            while (sample >= 1)
            {
                var a = random.NextInt32();
                var b = random.NextInt32();
                //The high bits of 0.9999999999999999999999999999m are 542101086.
                var c = random.Next(542101087);
                sample = new decimal(a, b, c, false, 28);
            }

            return sample;
        }

        /// <summary>
        /// Generates a random decimal number that is at least 0 and less than a given upper bound.
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random decimal number from the interval [0, maxValue).</returns>
        public static decimal NextDecimal(this Random random, decimal maxValue)
        {
            return NextDecimal(random, decimal.Zero, maxValue);
        }

        /// <summary>
        /// Generates a random decimal number that is at least a given lower bound and less than a given upper bound.
        /// </summary>
        /// <param name="random">The random.</param>
        /// <param name="minValue">The lower bound.</param>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random decimal number from the interval [minValue, maxValue).</returns>
        public static decimal NextDecimal(this Random random, decimal minValue, decimal maxValue)
        {
            var nextDecimalSample = NextDecimal(random);
            return maxValue * nextDecimalSample + minValue * (1 - nextDecimalSample);
        }
    }
}