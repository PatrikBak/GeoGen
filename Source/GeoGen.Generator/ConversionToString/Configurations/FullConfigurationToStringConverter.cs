using GeoGen.Core;
using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IToStringConverter{T}"/>, where 'T' is <see cref="GeneratedConfiguration"/>,
    /// that converts a configuration to the string represeting its minimal version 
    /// (see the documentation of <see cref="IMinimalFormResolver"/>). This class adapts
    /// more general <see cref="IConfigurationToStringProvider"/> interface that requires
    /// <see cref="IObjectToStringConverter"/> in order to convert a configuration to string.
    /// This converters are gotten from the <see cref="IFullObjectToStringConvertersFactory"/>
    /// according to the correct minimal resolver of the configuration. Therefore two configuration
    /// that are resolved to be isomorphic will be converted to the same string (which is 
    /// what the algorithm needs, since we want to rule out isomorphic configurations).
    /// </summary>
    public class FullConfigurationToStringConverter : IToStringConverter<GeneratedConfiguration>
    {
        #region Dependencies

        /// <summary>
        /// The generic configuration to string provider.
        /// </summary>
        private readonly IConfigurationToStringProvider _provider;

        private readonly IFullObjectToStringConvertersContainer _container;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="factory">The factory used to find the right object to string converters.</param>
        /// <param name="provider">The generic provider used to delegate the actual conversion to.</param>
        public FullConfigurationToStringConverter(IFullObjectToStringConvertersContainer container, IConfigurationToStringProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        #endregion

        #region  IToStringConverter implementation

        /// <summary>
        /// Converts a given configuration to string.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The string representation.</returns>
        public string ConvertToString(GeneratedConfiguration configuration)
        {
            // Initialize the minimal string
            string minimalString = null;

            // Determine the lexicographically least string representing the converted configuration using all possible converters
            foreach (var converter in _container)
            {
                // Convert a given configuration to string using the gotten converter
                var stringVersion = _provider.ConvertToString(configuration, converter);

                // If it's the first conversion or we have a smaller string...
                if (minimalString == null || string.CompareOrdinal(minimalString, stringVersion) < 0)
                {
                    // Set the minimal string 
                    minimalString = stringVersion;
                }
            }

            // And finally return the result
            return minimalString;
        }
       
        #endregion
    }
}