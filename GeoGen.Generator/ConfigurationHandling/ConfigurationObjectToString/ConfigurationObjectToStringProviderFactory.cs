using System;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
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
        /// with a given configuration object id resolver. The resolver cannot be default.
        /// </summary>
        /// <param name="resolver">The configuration object id resolver.</param>
        /// <returns>The custom complex configuration objct to string provider.</returns>
        public CustomComplexConfigurationObjectToStringProvider CreateCustomProvider(IObjectIdResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            return new CustomComplexConfigurationObjectToStringProvider(_argumentsToStringProvider, resolver);
        }

        /// <summary>
        /// Creates an instance of <see cref="DefaultComplexConfigurationObjectToStringProvider"/>
        /// with a given configuration object id resolver.
        /// </summary>
        /// <param name="resolver">The configuration object id resolver.</param>
        /// <returns>The default configuration objct to string provider.</returns>
        public DefaultConfigurationObjectToStringProvider CreateDefaltProvider(IObjectIdResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            return new DefaultConfigurationObjectToStringProvider(resolver);
        }

        #endregion
    }
}