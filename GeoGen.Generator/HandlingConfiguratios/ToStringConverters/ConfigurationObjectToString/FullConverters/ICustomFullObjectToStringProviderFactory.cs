namespace GeoGen.Generator
{
    /// <summary>
    /// An abstract factory for creating <see cref="CustomFullObjectToStringConverter"/>
    /// objects that use a <see cref="DictionaryObjectIdResolver"/>.
    /// </summary>
    internal interface ICustomFullObjectToStringConverterFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="CustomFullObjectToStringConverter"/>
        /// that uses a given dictionary object id resolver as its id resolver.
        /// </summary>
        /// <param name="resolver">The dictionary object id resolver.</param>
        /// <returns>The custom full object to string provider.</returns>
        CustomFullObjectToStringConverter GetCustomProvider(DictionaryObjectIdResolver resolver);
    }
}