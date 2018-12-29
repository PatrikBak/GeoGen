using System;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A static class containing helper method for ensuring that values passed as arguments meet certain conditions.
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Makes sure the passed number is greater than zero.
        /// </summary>
        /// <param name="number">The passed number.</param>
        /// <param name="argumentName">The name of the initial passed argument.</param>
        public static void IsGreaterThanZero(int number, string argumentName)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(argumentName, number, "The passed number should be greater than zero");
        }
    }
}
