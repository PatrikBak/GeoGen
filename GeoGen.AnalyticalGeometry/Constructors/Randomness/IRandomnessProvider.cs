using System;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// Represents a needed functionality of a <see cref="Random"/> object.
    /// The reason for having this as an interface is that we can mock it
    /// to have deterministic unit tests.
    /// </summary>
    public interface IRandomnessProvider
    {
        /// <summary>
        /// Generates a random decimal number that is at least 0 and less than 1.
        /// </summary>
        /// <returns>A random decimal number from the interval [0,1).</returns>
        decimal NextDecimal();

        /// <summary>
        /// Generates a random decimal number that is at least a given lower bound and less than a given upper bound.
        /// </summary>
        /// <param name="minValue">The lower bound.</param>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random decimal number from the interval [minValue, maxValue).</returns>
        decimal NextDecimal(decimal minValue, decimal maxValue);

        /// <summary>
        /// Generates a random decimal number that is at least 0 and less than a given upper bound.
        /// </summary>
        /// <param name="maxValue">The upper bound.</param>
        /// <returns>A random decimal number from the interval [0, maxValue).</returns>
        decimal NextDecimal(decimal maxValue);
    }
}