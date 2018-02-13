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
        /// Generates a random decimal number that is at least 0 and less than 1.
        /// </summary>
        /// <returns>A random decimal number from the interval [0,1).</returns>
        public decimal NextDecimal() => _random.NextDecimal();

        /// <summary>
        /// Generates a random decimal number that is at least a given lower bound and less than a given upper bound.
        /// </summary>
        /// <param name="minValue">The lower bound.</param>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random decimal number from the interval [minValue, maxValue).</returns>
        public decimal NextDecimal(decimal minValue, decimal maxValue) => _random.NextDecimal(minValue, maxValue);

        /// <summary>
        /// Generates a random decimal number that is at least 0 and less than a given upper bound.
        /// </summary>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random decimal number from the interval [0, maxValue).</returns>
        public decimal NextDecimal(decimal maxValue) => _random.NextDecimal(maxValue);

        #endregion
    }
}