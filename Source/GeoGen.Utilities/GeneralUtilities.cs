using System;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A static helper class for general C# utilities
    /// </summary>
    public static class GeneralUtilities
    {
        /// <summary>
        /// Swaps the values of two elements, given by its references.
        /// </summary>
        /// <param name="o1">The reference of the first element.</param>
        /// <param name="o2">The reference of the second element.</param>
        public static void Swap<T>(ref T o1, ref T o2)
        {
            var tmp = o1;
            o1 = o2;
            o2 = tmp;
        }

        /// <summary>
        /// Executes a given action a given number of times.
        /// </summary>
        /// <param name="numberOfTimes">The number of times to execute the action.</param>
        /// <param name="action">The action.</param>
        public static void ExecuteNTimes(int numberOfTimes, Action action)
        {
            if (numberOfTimes < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfTimes), $"The parameter should be non-negative, but it's {numberOfTimes}.");

            // For the given number of times
            for (var i = 0; i < numberOfTimes; i++)
            {
                // Perform the action
                action();
            }
        }
    }
}