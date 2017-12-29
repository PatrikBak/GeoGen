using System;

namespace GeoGen.Utilities.Helpers
{
    /// <summary>
    /// A helper class for throwing exceptions
    /// </summary>
    public static class ThrowHelper
    {
        /// <summary>
        /// Throws an expected when a given condition is not met.
        /// </summary>
        /// <param name="condition">The boolean condition.</param>
        public static void ThrowExceptionIfNotTrue(bool condition)
        {
            if (!condition)
                throw new Exception();
        }
    }
}