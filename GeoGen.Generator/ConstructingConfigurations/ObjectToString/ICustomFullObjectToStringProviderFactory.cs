using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectToString
{
    /// <summary>
    /// An abstract factory for creating <see cref="CustomFullObjectToStringProvider"/>
    /// objects.
    /// </summary>
    internal interface ICustomFullObjectToStringProviderFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="CustomFullObjectToStringProvider"/>
        /// that uses a given dictionary object id resolver as its id resolver.
        /// </summary>
        /// <param name="resolver">The dictionary object id resolver.</param>
        /// <returns>The custom full object to string provider.</returns>
        CustomFullObjectToStringProvider GetCustomProvider(DictionaryObjectIdResolver resolver);
    }
}