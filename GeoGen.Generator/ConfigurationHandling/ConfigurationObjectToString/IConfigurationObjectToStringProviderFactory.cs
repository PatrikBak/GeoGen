using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An abstract factory for creating implementations of 
    /// <see cref="IConfigurationObjectToStringProvider"/>.
    /// </summary>
    internal interface IConfigurationObjectToStringProviderFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="CustomComplexConfigurationObjectToStringProvider"/>
        /// with a given configuration object id resolver. The resolver cannot be default.
        /// </summary>
        /// <param name="resolver">The configuration object id resolver.</param>
        /// <returns>The custom complex configuration objct to string provider.</returns>
        CustomComplexConfigurationObjectToStringProvider CreateCustomProvider(IObjectIdResolver resolver);

        /// <summary>
        /// Creates an instance of <see cref="DefaultComplexConfigurationObjectToStringProvider"/>
        /// with a given configuration object id resolver.
        /// </summary>
        /// <param name="resolver">The configuration object id resolver.</param>
        /// <returns>The default configuration objct to string provider.</returns>
        DefaultConfigurationObjectToStringProvider CreateDefaltProvider(IObjectIdResolver resolver);
    }
}