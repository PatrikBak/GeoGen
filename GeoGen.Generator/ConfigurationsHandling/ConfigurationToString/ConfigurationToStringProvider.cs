using System;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationToString
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationToStringProvider"/>.
    /// </summary>
    internal class ConfigurationToStringProvider : IConfigurationToStringProvider
    {
        #region Private fields

        /// <summary>
        /// The default separator.
        /// </summary>
        private const string DefaultSeparator = "|";

        /// <summary>
        /// The separator.
        /// </summary>
        private readonly string _separator;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new configuration to string provider with
        /// a given separator. This is meant to be used during 
        /// unit testing.
        /// </summary>
        /// <param name="separator">The separator.</param>
        public ConfigurationToStringProvider(string separator)
        {
            _separator = separator;
        }

        /// <summary>
        /// Constructs a new configuration to string provider 
        /// with a default separator.
        /// </summary>
        public ConfigurationToStringProvider()
            : this(DefaultSeparator)
        {
        }

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

            objectStrings.Sort();

            return string.Join(_separator, objectStrings);
        }

        #endregion
    }
}