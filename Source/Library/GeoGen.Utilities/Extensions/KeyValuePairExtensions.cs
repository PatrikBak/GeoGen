namespace GeoGen.Utilities
{
    /// <summary>
    /// The extension methods for <see cref="KeyValuePair{TKey, TValue}"/>.
    /// </summary>
    public static class KeyValuePairExtensions
    {
        /// <summary>
        /// Deconstructs the <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="pair">The key-value pair.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            // Do the deconstruction
            key = pair.Key;
            value = pair.Value;
        }
    }
}
