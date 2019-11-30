using System.Linq;
using System.Text;

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

        /// <summary>
        /// Replaces every comma that is not between unbalanced braces of type () or [] with a semicolon. For example:
        /// A, B, C(1, 5), D[1, 2, E(5, 6)] will become A; B; C(1, 5); D[1, 2, E(5, 6)]
        /// </summary>
        /// <param name="string">The string.</param>
        /// <returns>The string in which all commas within unbalanced braces have been replaced with semicolons.</returns>
        public static string ReplaceBalancedCommasWithSemicolons(this string @string)
        {
            // Copy the string so we can edit it
            var stringCopy = new StringBuilder(@string);

            // Prepare the number of unclosed brackets
            var unclosedBrackets = 0;

            // Go through the characters
            @string.ForEach((c, index) =>
            {
                // If we have an open bracket, count it in
                if (c == '(' || c == '[')
                    unclosedBrackets++;

                // If we have a closed one, count it out
                else if (c == ')' || c == ']')
                    unclosedBrackets--;

                // If we have a comma and there are no unclosed brackets, replace it with a semicolon
                else if (c == ',' && unclosedBrackets == 0)
                    stringCopy[index] = ';';
            });

            // Return the final string
            return stringCopy.ToString();
        }
    }
}