using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString
{
    /// <summary>
    /// A default implementation of <see cref="IObjectToStringProvider"/>
    /// that simply converts an object's id to string.
    /// </summary>
    internal class DefaultObjectToStringProvider : IObjectToStringProvider
    {
        #region IObjectToStringProvider properties

        /// <summary>
        /// Gets the id of the provider.
        /// </summary>
        public int Id => 0;

        #endregion

        #region IObjectToStringProvider methods

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

            return configurationObject.Id.ToString();
        }

        #endregion
    }
}