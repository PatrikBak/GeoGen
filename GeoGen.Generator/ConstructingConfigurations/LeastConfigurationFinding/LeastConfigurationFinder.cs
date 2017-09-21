using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding
{
    /// <summary>
    /// An implementation of <see cref="ILeastConfigurationFinder"/> that detects
    /// formal configurations symmetry. For example: [A, B, C, midpoint(A, B)] is 
    /// symmetric to [A, B, C, midpoint(B, C)]. The 'formal' definition of this symmetry
    /// could be: Configuration C1 is symmetric to configuration C2 if and only if
    /// there isn't a permutation of loose points of C1 that after applying to C2
    /// yields the same configuration as C1. This class is not thread-safe.
    /// </summary>
    internal class LeastConfigurationFinder : ILeastConfigurationFinder
    {
        #region Private fields

        /// <summary>
        /// The configuration to string provider.
        /// </summary>
        private readonly IConfigurationToStringProvider _configurationToString;

        /// <summary>
        /// The custom full object to string provider factory.
        /// </summary>
        private readonly ICustomFullObjectToStringProviderFactory _objectToStringFactory;

        /// <summary>
        /// The dictionary object id resolvers container.
        /// </summary>
        private readonly IDictionaryObjectIdResolversContainer _dictionaryIdResolversContainer;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new least configuration finder that uses
        /// a configuration to string provider, a full object to string
        /// provider factory (that is used for getting full object to string
        /// providers to be passed during configuration to string conversion)
        /// and a container of all dictionary object id resolvers.
        /// </summary>
        /// <param name="configurationToString">The configuration to string provider.</param>
        /// <param name="objectToStringFactory">The custom full object to string provider factory.</param>
        /// <param name="dictionaryIdResolversContainer">The dictionary object id resolvers container. </param>
        public LeastConfigurationFinder
        (
            IConfigurationToStringProvider configurationToString,
            ICustomFullObjectToStringProviderFactory objectToStringFactory,
            IDictionaryObjectIdResolversContainer dictionaryIdResolversContainer
        )
        {
            _configurationToString = configurationToString ?? throw new ArgumentNullException(nameof(configurationToString));
            _objectToStringFactory = objectToStringFactory ?? throw new ArgumentNullException(nameof(objectToStringFactory));
            _dictionaryIdResolversContainer = dictionaryIdResolversContainer ?? throw new ArgumentNullException(nameof(dictionaryIdResolversContainer));
        }

        #endregion

        #region ILeastConfigurationFinder methods

        /// <summary>
        /// Finds the 'least' configuration representant of the equivalency
        /// class specified by a given configuration. This equivalency class
        /// is represented as a <see cref="DictionaryObjectIdResolver"/>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The dictionary object id resolver.</returns>
        public DictionaryObjectIdResolver FindLeastConfiguration(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Prepare variables
            string leastString = null;
            DictionaryObjectIdResolver result = null;

            // Iterate over all the dictionary id resolvers
            foreach (var resolver in _dictionaryIdResolversContainer)
            {
                // For a given get the custom object to string provider from the factory
                var customProvider = _objectToStringFactory.GetCustomProvider(resolver);

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