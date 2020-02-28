using GeoGen.Utilities;

namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// The extension methods for <see cref="double"/>.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Convert given value to a string that is understand by MetaPost.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <returns></returns>
        public static string ToStringReadableByMetapost(this double value) =>
            // We need to use 339 digits because that's the maximal number of digits a double can have
            // We also need to use the invariant string to make sure we get the decimal point
            value.ToStringWithDecimalDot(numberOfDecimalPlaces: 339);
    }
}