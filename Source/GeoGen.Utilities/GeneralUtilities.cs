using System;
using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// General C# static utilities
    /// </summary>
    public static class GeneralUtilities
    {
        /// <summary>
        /// Swaps the values of two elements, given by their references.
        /// </summary>
        /// <param name="o1">The reference to the first element.</param>
        /// <param name="o2">The reference to the second element.</param>
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
        /// <param name="action">The action to be executed.</param>
        public static void ExecuteNTimes(int numberOfTimes, Action action)
        {
            // For the given number of times
            for (var i = 0; i < numberOfTimes; i++)
            {
                // Perform the action
                action();
            }
        }

        /// <summary>
        /// Executes a given function a given number of times and returns the results.
        /// </summary>
        /// <param name="numberOfTimes">The number of times to execute the function.</param>
        /// <param name="function">The function to be executed.</param>
        public static IEnumerable<T> ExecuteNTimes<T>(int numberOfTimes, Func<T> function)
        {
            // For the given number of times
            for (var i = 0; i < numberOfTimes; i++)
            {
                // Execute the function
                yield return function();
            }
        }

        /// <summary>
        /// Safely executes a given function while catching an exception of given type and handling it.
        /// </summary>
        /// <typeparam name="TResult">The return type of the function.</typeparam>
        /// <typeparam name="TException">The type of exception to be caught.</typeparam>
        /// <param name="function">The function to be executed.</param>
        /// <param name="exceptionHandler">The handler for the exception of given type.</param>
        /// <returns>Either the result of the function, if there is no exception, of the default value.</returns>
        public static TResult TryExecute<TResult, TException>(Func<TResult> function, Action<TException> exceptionHandler)
            where TException : Exception
        {
            try
            {
                // Try to call the function
                return function();
            }
            catch (TException e)
            {
                // Handle the exception
                exceptionHandler(e);

                // Return the default value
                return default;
            }
        }
    }
}