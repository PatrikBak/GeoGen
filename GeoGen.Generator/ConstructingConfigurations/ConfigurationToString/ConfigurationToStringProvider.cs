using System;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;

namespace GeoGen.Generator.ConstructingConfigurations.ConfigurationToString
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationToStringProvider"/>.
    /// It simply converts each object to string and join these strings using the default
    /// separator. This class is thread-safe.
    /// </summary>
    internal class ConfigurationToStringProvider : IConfigurationToStringProvider
    {
        #region Private constants

        /// <summary>
        /// The default separator.
        /// </summary>
        private const string DefaultSeparator = "|";

        #endregion

        #region IConfigurationToStringProvider implementation

        /// <summary>
        /// Converts a given configuration to string, using a given 
        /// configuration object to string provider.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the configuration.</returns>
        public string ConvertToString(Configuration configuration, IObjectToStringProvider objectToString)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (objectToString == null)
                throw new ArgumentNullException(nameof(objectToString));

            var objectStrings = configuration.ConstructedObjects
                    .Select(objectToString.ConvertToString)
                    .ToList();

            // TODO: This can be replaced with iterating over some SortedDictionary
            objectStrings.Sort();

            return string.Join(DefaultSeparator, objectStrings);
        }

        #endregion
    }
}