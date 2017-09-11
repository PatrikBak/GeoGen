using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationObjectToStringProvider"/>
    /// that uses the configuration Id to string method.
    /// </summary>
    internal class DefaultConfigurationObjectIdToStringProvider : IConfigurationObjectToStringProvider
    {
        #region Instance

        /// <summary>
        /// The single isntance of this class.
        /// </summary>
        public static readonly DefaultConfigurationObjectIdToStringProvider Instance = new DefaultConfigurationObjectIdToStringProvider();

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

            return configurationObject.Id.Value.ToString();
        }

        #endregion
    }
}