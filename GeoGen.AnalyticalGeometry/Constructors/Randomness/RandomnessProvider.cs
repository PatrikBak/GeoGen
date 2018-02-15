using System;
using GeoGen.Utilities;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// A default implementation of <see cref="IRandomObjectsProvider"/>.
    /// </summary>
    public class RandomnessProvider : IRandomnessProvider
    {
        #region Private fields

        /// <summary>
        /// The random object.
        /// </summary>
        private readonly Random _random = new Random();

        #endregion

        #region IRandomnessProvider implementation

        /// <summary>
        /// Generates a random double number that is at least 0 and less than 1.
        /// </summary>
        /// <returns>A random double number from the interval [0,1).</returns>
        public double NextDouble() => _random.NextDouble();

        /// <summary>
        /// Generates a random double number that is at least a given lower bound and less than a given upper bound.
        /// </summary>
        /// <param name="minValue">The lower bound.</param>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random double number from the interval [minValue, maxValue).</returns>
        public double NextDouble(double minValue, double maxValue) => minValue + (maxValue - minValue) * _random.NextDouble();

        /// <summary>
        /// Generates a random double number that is at least 0 and less than a given upper bound.
        /// </summary>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random double number from the interval [0, maxValue).</returns>
        public double NextDouble(double maxValue) => maxValue * _random.NextDouble();

        #endregion
    }
}