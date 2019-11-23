using System.Linq;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Adds a given number of spaces at the beginning of each line of the string.
        /// </summary>
        /// <param name="string">The string.</param>
        /// <param name="numberOfSpaces">The number of spaces added at the beginning of each line.</param>
        /// <returns>The indented string.</returns>
        public static string Indent(this string @string, int numberOfSpaces)
            // Split the string, append spaces at the beginning, join again
            => @string.Split('\n').Select(s => $"{new string(' ', numberOfSpaces)}{s}").ToJoinedString("\n");
    }
}