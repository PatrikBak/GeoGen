using System;

namespace GeoGen.AnalyticalGeometry.RandomObjects
{
    /// <summary>
    /// A default implementation of <see cref="IRandomObjectsProvider"/>.
    /// </summary>
    internal sealed class RandomnessProvider : IRandomnessProvider
    {
        #region Private fields

        /// <summary>
        /// The random object.
        /// </summary>
        private readonly Random _random = new Random();

        #endregion

        #region IRandomnessProvider implementation

        /// <summary>
        /// Generates a double number in the range [0, upperBound)
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>The double.</returns>
        public double NextDouble(double upperBound)
        {
            return _random.NextDouble() * upperBound;
        }

        #endregion
    }
}