using System;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// Represents a needed functionality of a <see cref="Random"/> object.
    /// The reason for having this as an interface is that we can mock it
    /// to have reusable unit tests.
    /// </summary>
    public interface IRandomnessProvider
    {
        /// <summary>
        /// Generates a double number in the range [0, upperBound)
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns>The double.</returns>
        double NextDouble(double upperBound);
    }
}