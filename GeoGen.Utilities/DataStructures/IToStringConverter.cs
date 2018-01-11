namespace GeoGen.Utilities
{
    public interface IToStringConverter<in T>
    {
        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The string representation.</returns>
        string ConvertToString(T item);
    }
}
