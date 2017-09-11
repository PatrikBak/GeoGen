using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An abstract factory for creating implementations of 
    /// <see cref="ComplexConfigurationObjectToStringProviderBase"/>.
    /// </summary>
    internal interface IConfigurationObjectToStringProviderFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="CustomComplexConfigurationObjectToStringProvider"/>
        /// with a given loose configuration object id resolver. The resolver cannot be default.
        /// </summary>
        /// <param name="resolver">The loose configuration object id resolver</param>
        /// <returns>The custom complex configuration objct to string provider.</returns>
        CustomComplexConfigurationObjectToStringProvider CreateCustomProvider(ILooseConfigurationObjectIdResolver resolver);

        /// <summary>
        /// Creates an instance of <see cref="DefaultComplexConfigurationObjectToStringProvider"/>.
        /// </summary>
        /// <returns>The default complex configuration object to string provider.</returns>
        DefaultComplexConfigurationObjectToStringProvider CreateDefaultProvider();
    }
}