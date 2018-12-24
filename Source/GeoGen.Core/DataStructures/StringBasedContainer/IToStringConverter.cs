namespace GeoGen.Core
{
    /// <summary>
    /// Represents a converter of objects to string.
    /// </summary>
    /// <typeparam name="T">The type of object to be converted.</typeparam>
    public interface IToStringConverter<T>
    {
        /// <summary>
        /// Converts a given item to a string.
        /// </summary>
        /// <param name="item">The item to be converted.</param>
        /// <returns>A string representation of the item.</returns>
        string ConvertToString(T item);
    }
}