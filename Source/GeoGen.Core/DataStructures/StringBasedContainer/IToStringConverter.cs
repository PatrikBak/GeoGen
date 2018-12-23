namespace GeoGen.Core
{
    /// <summary>
    /// Represents a converter of objects to string.
    /// </summary>
    /// <typeparam name="T">The type of object to be converted.</typeparam>
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