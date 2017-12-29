namespace GeoGen.Utilities.Helpers
{
    /// <summary>
    /// General C# utilities
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Swaps the value of two elements.
        /// </summary>
        /// <param name="o1">The reference of the first element.</param>
        /// <param name="o2">The reference of the second element.</param>
        public static void Swap<T>(ref T o1, ref T o2)
        {
            var tmp = o1;
            o1 = o2;
            o2 = tmp;
        }
    }
}