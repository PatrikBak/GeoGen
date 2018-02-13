using System;
using System.Collections.Generic;
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

        /// <summary>
        /// The current id prepared to be set to a <see cref="ConfigurationWrapper"/>.
        /// </summary>
        private int _currentId;

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
        /// Constructs a configuration wrapper from a given constructor output.
        /// </summary>
        /// <param name="constructorOutput">The constructor output.</param>
        /// <returns>The wrapper of the new configuration.</returns>
        public ConfigurationWrapper ConstructWrapper(ConstructorOutput constructorOutput)
        {
            // Pull original configuration
            var originalConfiguration = constructorOutput.OriginalConfiguration.WrappedConfiguration;

            // Derive a new configuration
            var newConfiguration = originalConfiguration.Derive(constructorOutput.ConstructedObjects);

            // Create the new wrapper. The resolver to the minimal form will be found and set later
            var wrapper = new ConfigurationWrapper
            {
                Id = _currentId++,
                WrappedConfiguration = newConfiguration,
                PreviousConfiguration = constructorOutput.OriginalConfiguration,
                LastAddedObjects = constructorOutput.ConstructedObjects,
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
        /// <returns>The wrapper of the initial configuration.</returns>
        public ConfigurationWrapper ConstructInitialWrapper(Configuration initialConfiguration)
        {
            return new ConfigurationWrapper
            {
                Id = _currentId++,
                WrappedConfiguration = initialConfiguration,
                LastAddedObjects = new List<ConstructedConfigurationObject>(),
                PreviousConfiguration = null,
                ResolverToMinimalForm = null
            };
        }

        #endregion
    }
}