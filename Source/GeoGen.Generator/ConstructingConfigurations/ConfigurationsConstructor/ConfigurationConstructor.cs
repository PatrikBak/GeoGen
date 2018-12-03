using System;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationConstructor"/>. This class
    /// uses an <see cref="IMinimalFormResolver"/> to find out the minimal
    /// form of a configuration.
    /// </summary>
    internal class ConfigurationConstructor : IConfigurationConstructor
    {
        #region Private fields

        /// <summary>
        /// The resolver to minimal form of <see cref="ConfigurationWrapper"/>s. The resolved
        /// result will be set to the wrappers.
        /// </summary>
        private readonly IMinimalFormResolver _minimalFormResolver;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="resolver">The resolver to find the minimal form of a configuration.</param>
        public ConfigurationConstructor(IMinimalFormResolver resolver)
        {
            _minimalFormResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region IConfigurationConstructor methods

        /// <summary>
        /// Constructs a configuration wrapper from a new configuration to be wrapped 
        /// and the construction that was extended.
        /// </summary>
        /// <param name="newConfiguration">The configuration to be wrapped.</param>
        /// <param name="oldConfiguration">The old configuration that was extended.</param>
        /// <returns>The wrapper of the configuration.</returns>
        public ConfigurationWrapper ConstructWrapper(Configuration newConfiguration, ConfigurationWrapper oldConfiguration)
        {
            // Create the new wrapper. The resolver to the minimal form will be found and set later
            var wrapper = new ConfigurationWrapper
            {
                WrappedConfiguration = newConfiguration,
                PreviousConfiguration = oldConfiguration,
            };

            // Let the resolver find its resolver to minimal form
            var leastResolver = _minimalFormResolver.FindResolverToMinimalForm(wrapper);

            // Set the resolver
            wrapper.ResolverToMinimalForm = leastResolver;

            // And return the result
            return wrapper;
        }

        /// <summary>
        /// Constructs a configuration wrapper from a given initial configuration.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <returns>The wrapper of the configuration.</returns>
        public ConfigurationWrapper ConstructInitialWrapper(Configuration initialConfiguration)
        {
            return new ConfigurationWrapper
            {
                WrappedConfiguration = initialConfiguration,
                PreviousConfiguration = null,
                ResolverToMinimalForm = null
            };
        }

        #endregion
    }
}