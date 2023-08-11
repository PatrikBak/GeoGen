namespace GeoGen.Utilities
{
    /// <summary>
    /// A static helper class for generating random numbers. This class is thread-safe, which is achieved by locking.
    /// Therefore, for performance-critical generated it is better to use custom <see cref="Random"/> objects.
    /// </summary>
    public static class RandomnessHelper
    {
        #region Instance

        /// <summary>
        /// The instance of the random numbers generator.
        /// </summary>
        private static readonly Random _random = new Random();

        #endregion

        #region Public methods

        /// <summary>
        /// Generates a random number that is at least a given lower bound and less than a given upper bound.
        /// </summary>
        /// <param name="minValue">The lower bound.</param>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random double number from the interval [minValue, maxValue).</returns>
        public static double NextDouble(double minValue, double maxValue)
        {
            // Make sure 2 thread won't call this at the same time, 
            // since the Random object is not thread-safe
            lock (_random)
            {
                // NextDouble() is in the interval [0,1), we make it from [minValue, maxValue)
                return minValue + (maxValue - minValue) * _random.NextDouble();
            }
        }

        #endregion
    }
}