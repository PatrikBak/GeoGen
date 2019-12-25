using System.Globalization;

namespace GeoGen.Drawer
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
            // We also need to use the invariant culture to make sure we get the decimal point
            value.ToString($"0.{new string('#', 339)}", CultureInfo.InvariantCulture);
    }
}