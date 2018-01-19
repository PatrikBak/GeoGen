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
    }
}