using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An abstract factory for creating implementations of <see cref="CustomFullObjectToStringProvider"/>.
    /// </summary>
    internal interface ICustomFullObjectToStringProviderFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="CustomFullObjectToStringProvider"/>
        /// with a given dictionary object id resolver.
        /// </summary>
        /// <param name="resolver">The dictionary object id resolver.</param>
        /// <returns>The custom full object to string provider.</returns>
        CustomFullObjectToStringProvider GetCustomProvider(DictionaryObjectIdResolver resolver);
    }
}