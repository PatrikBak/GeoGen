using System;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IMinimalFormResolver"/>.
    /// This minimal form is found by converting a configuration to string using
    /// all possible <see cref="IObjectIdResolver"/>s representing a permutation
    /// of <see cref="LooseConfigurationObject"/>, and then taking the lexicographically least
    /// configuration. This will work because of the nature of the full configuration to string
    /// conversion. It can be mathematically proved that two symmetric configuration will 
    /// have these least representations the same.  
    /// </summary>
    internal class MinimalFormResolver : IMinimalFormResolver
    {
        #region Private fields

        /// <summary>
        /// The converter of configurations to string that is used together with 
        /// <see cref="IFullObjectToStringConverter"/>s. 
        /// </summary>
        private readonly IConfigurationToStringProvider _toStringProvider;

        /// <summary>
        /// The containers of <see cref="IObjectIdResolver"/> that represent
        /// the permutations of ids of the <see cref="LooseConfigurationObject"/>s.
        /// </summary>
        private readonly IObjectIdResolversContainer _container;

        /// <summary>
        /// The factory for getting implementations of <see cref="IFullObjectToStringConverter"/>s
        /// to be passed to the <see cref="IConfigurationToStringProvider"/>.
        /// </summary>
        private readonly IFullObjectToStringConvertersFactory _factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The configuration to string provider used to find the configuration with the least string form.</param>
        /// <param name="factory">The factory to get the full object to string converters for the configuration converter.</param>
        /// <param name="container">The containers of all ids resolvers to be passed to the converters factory.</param>
        public MinimalFormResolver(IConfigurationToStringProvider provider, IFullObjectToStringConvertersFactory factory, IObjectIdResolversContainer container)
        {
            _toStringProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region ILeastConfigurationFinder methods

        /// <summary>
        /// Find the resolver of a given configuration to its minimal form. For more information,
        /// see the documentation of <see cref="IMinimalFormResolver"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The resolver.</returns>
        public IObjectIdResolver FindResolverToMinimalForm(ConfigurationWrapper configuration)
        {
            // Initialize the result
            IObjectIdResolver result = null;

            // And the minimal string
            string minimalString = null;

            // Determine the lexicographically least string representing a converted configuration
            // using all ids resolvers
            foreach (var resolver in _container)
            {
                // For a given resolver get the full object to string converter from the factory
                var converter = _factory.Get(resolver);

                // Convert a given configuration to string using the gotten converter
                var stringVersion = _toStringProvider.ConvertToString(configuration, converter);

                // If it's the first conversion or we have a smaller string
                if (result == null || string.CompareOrdinal(minimalString, stringVersion) < 0)
                {
                    // Then the current result is the resolver
                    result = resolver;

                    // And the minimal string 
                    minimalString = stringVersion;
                }
            }

            // And finally return the result
            return result;
        }

        #endregion
    }
}