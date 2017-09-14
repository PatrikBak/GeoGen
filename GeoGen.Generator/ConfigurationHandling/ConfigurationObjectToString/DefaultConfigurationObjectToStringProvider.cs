using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationObjectToStringProvider"/>
    /// that uses the configuration Id to string method. It might be used
    /// with a custom <see cref="IObjectIdResolver"/>.
    /// </summary>
    internal class DefaultConfigurationObjectToStringProvider : IConfigurationObjectToStringProvider
    {
        #region Private fields

        /// <summary>
        /// The configuration object id resolver.
        /// </summary>
        private readonly IObjectIdResolver _objectIdResolver;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a default configuration object to string
        /// provider with a given configuration object id resolver.
        /// </summary>
        /// <param name="objectIdResolver">The configuration object id resolver.</param>
        public DefaultConfigurationObjectToStringProvider(IObjectIdResolver objectIdResolver)
        {
            _objectIdResolver = objectIdResolver ?? throw new ArgumentNullException(nameof(objectIdResolver));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DefaultConfigurationObjectToStringProvider()
        {
        }

        #endregion

        #region IConfigurationObjectToStringProvider implementation

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            if (!configurationObject.Id.HasValue)
                throw new GeneratorException("The configuration object doesn't have an id.");

            var id = configurationObject.Id.Value;

            return _objectIdResolver?.ResolveId(configurationObject).ToString() ?? id.ToString();
        }

        #endregion
    }
}