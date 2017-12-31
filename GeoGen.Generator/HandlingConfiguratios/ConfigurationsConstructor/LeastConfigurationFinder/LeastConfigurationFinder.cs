using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// An implementation of <see cref="ILeastConfigurationFinder"/> that detects
    /// formal configurations symmetry. For example: [A, B, C, midpoint(A, B)] is 
    /// symmetric to [A, B, C, midpoint(B, C)]. The 'formal' definition of this symmetry
    /// could be: Configuration C1 is symmetric to configuration C2 if and only if
    /// there isn't a permutation of loose points of C1 that after applying to C2
    /// yields the same configuration as C1. This sealed class is not thread-safe.
    /// </summary>
    internal sealed class LeastConfigurationFinder : ILeastConfigurationFinder
    {
        #region Private fields

        /// <summary>
        /// The configuration to string provider.
        /// </summary>
        private readonly IConfigurationToStringProvider _configurationToString;

        /// <summary>
        /// The dictionary object id resolvers container.
        /// </summary>
        private readonly IDictionaryObjectIdResolversContainer _dictionaryIdResolversContainer;

        private readonly IFullObjectToStringConverterFactory _factory;

        private readonly IDefaultObjectIdResolver _defaultResolver;
        
        #endregion

        #region Constructor

        public LeastConfigurationFinder(IConfigurationToStringProvider configurationToString, IDictionaryObjectIdResolversContainer dictionaryIdResolversContainer, IFullObjectToStringConverterFactory factory, IDefaultObjectIdResolver defaultResolver)
        {
            _configurationToString = configurationToString;
            _dictionaryIdResolversContainer = dictionaryIdResolversContainer;
            _factory = factory;
            _defaultResolver = defaultResolver;
        }

        #endregion

        #region ILeastConfigurationFinder methods

        /// <summary>
        /// Finds the 'least' configuration representant of the equivalency
        /// sealed class specified by a given configuration. This equivalency sealed class
        /// is represented as a <see cref="DictionaryObjectIdResolver"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The dictionary object id resolver.</returns>
        public IObjectIdResolver FindLeastConfiguration(ConfigurationWrapper configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Prepare result
            IObjectIdResolver result = _defaultResolver;

            // Prepare variables
            var leastString = _configurationToString.ConvertToString(configuration, _factory.Get(_defaultResolver));

            // Iterate over all the dictionary id resolvers
            foreach (var resolver in _dictionaryIdResolversContainer.GetNonIdenticalResolvers())
            {
                // For a given get the custom object to string provider from the factory
                var customProvider = _factory.Get(resolver);

                // Convert a given configuration to string using the gotten resolver
                var stringVersion = _configurationToString.ConvertToString(configuration, customProvider);

                // Compare the string the currently least version
                var lessThanLeast = string.Compare(stringVersion, leastString, StringComparison.Ordinal) < 0;

                // If the least string hasn't been set (i.e. it's the first iteration)
                // or we have found a 'less' string
                if (leastString == null || lessThanLeast)
                {
                    // Then we update the current minimal string and corresponding resolver
                    leastString = stringVersion;
                    result = resolver;
                }
            }

            return result;
        }

        #endregion
    }
}