using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString
{
    /// <summary>
    /// A default implementation of <see cref="IObjectToStringProvider"/>
    /// that uses <see cref="DefaultObjectIdResolver"/>.
    /// </summary>
    internal class DefaultObjectToStringProvider : ObjectToStringProviderBase
    {
        #region Constructor

        /// <summary>
        /// Constructs a new default object to string provider with a given
        /// default object id resolver.
        /// </summary>
        /// <param name="defaultResolver">The default object id resolver.</param>
        public DefaultObjectToStringProvider(DefaultObjectIdResolver defaultResolver)
            : base(defaultResolver)
        {
        }

        #endregion

        #region ObjectToStringProviderBase methods

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        public override string ConvertToString(ConfigurationObject configurationObject)
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