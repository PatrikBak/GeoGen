using System;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An implementation of <see cref="IConfigurationObjectToStringProviderFactory"/>.
    /// </summary>
    internal class ConfigurationObjectToStringProviderFactory : IConfigurationObjectToStringProviderFactory
    {
        #region Private fields

        /// <summary>
        /// The arguments to string provider.
        /// </summary>
        private readonly IArgumentsToStringProvider _argumentsToStringProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration object to string provider factory
        /// with a given arguments to string provider.
        /// </summary>
        /// <param name="argumentsToStringProvider">The arguments to string provider.</param>
        public ConfigurationObjectToStringProviderFactory(IArgumentsToStringProvider argumentsToStringProvider)
        {
            _argumentsToStringProvider = argumentsToStringProvider ?? throw new ArgumentNullException(nameof(argumentsToStringProvider));
        }

        #endregion

        #region IConfigurationObjectToStringProviderFactory implementation

        /// <summary>
        /// Creates an instance of <see cref="CustomComplexConfigurationObjectToStringProvider"/>
        /// with a given loose configuration object id resolver. The resolver cannot be default.
        /// </summary>
        /// <param name="resolver">The loose configuration object id resolver</param>
        /// <returns>The custom complex configuration objct to string provider.</returns>
        public CustomComplexConfigurationObjectToStringProvider CreateCustomProvider(ILooseConfigurationObjectIdResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            if (resolver is DefaultLooseConfigurationObjectIdResolver)
                throw new GeneratorException("This implementation is not supposed to used a default resolver");

            return new CustomComplexConfigurationObjectToStringProvider(_argumentsToStringProvider, resolver);
        }

        /// <summary>
        /// Creates an instance of <see cref="DefaultComplexConfigurationObjectToStringProvider"/>.
        /// </summary>
        /// <returns>The default complex configuration object to string provider.</returns>
        public DefaultComplexConfigurationObjectToStringProvider CreateDefaultProvider()
        {
            return new DefaultComplexConfigurationObjectToStringProvider(_argumentsToStringProvider);
        } 

        #endregion
    }
}