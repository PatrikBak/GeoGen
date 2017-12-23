using System;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectToString
{
    /// <summary>
    /// A default implementation of <see cref="IObjectToStringProvider"/>
    /// that uses a <see cref="DefaultObjectIdResolver"/>.
    /// </summary>
    internal sealed class DefaultObjectToStringProvider : IObjectToStringProvider
    {
        #region IObjectToStringProvider properties

        /// <summary>
        /// Gets the object to string resolver that is used by this provider.
        /// </summary>
        public IObjectIdResolver Resolver { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new default object to string provider with a given
        /// default object id resolver.
        /// </summary>
        /// <param name="defaultResolver">The default object id resolver.</param>
        public DefaultObjectToStringProvider(DefaultObjectIdResolver defaultResolver)
        {
            Resolver = defaultResolver ?? throw new ArgumentNullException(nameof(defaultResolver));
        }

        #endregion

        #region ObjectToStringProviderBase methods

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            if (!configurationObject.Id.HasValue)
                throw new GeneratorException("The configuration object doesn't have an id.");

            return Resolver.ResolveId(configurationObject).ToString();
        }

        #endregion
    }
}