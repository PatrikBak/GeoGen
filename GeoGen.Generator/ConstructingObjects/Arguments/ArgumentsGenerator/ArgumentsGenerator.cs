using System;
using System.Linq;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of the <see cref="IArgumentsGenerator"/> interface.
    /// </summary>
    internal sealed class ArgumentsGenerator : IArgumentsGenerator
    {
        #region Private fields

        /// <summary>
        /// The construction signature matcher.
        /// </summary>
        private readonly IConstructionSignatureMatcher _signatureMatcher;

        /// <summary>
        /// The arguments list container factory.
        /// </summary>
        private readonly IArgumentsListContainerFactory _argumentsListContainerFactory;

        /// <summary>
        /// The variations provider.
        /// </summary>
        private readonly IVariationsProvider _variationsProvider;

        /// <summary>
        /// The combinator.
        /// </summary>
        private readonly ICombinator _combinator;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new arguments generator. The generator uses combinator and 
        /// variations provider to create all possible sets of objects to be passed
        /// as arguments. These arguments are created by the construction signature matcher
        /// and then kept in an arguments container so that they we provide only
        /// distinct set of arguments.
        /// </summary>
        /// <param name="combinator">The combinator.</param>
        /// <param name="signatureMatcher">The construction signature matcher.</param>
        /// <param name="variationsProvider">The variations provider.</param>
        /// <param name="argumentsListContainerFactory">The arguments list container factory.</param>
        public ArgumentsGenerator
        (
            ICombinator combinator,
            IConstructionSignatureMatcher signatureMatcher,
            IVariationsProvider variationsProvider,
            IArgumentsListContainerFactory argumentsListContainerFactory
        )
        {
            _combinator = combinator ?? throw new ArgumentNullException(nameof(combinator));
            _signatureMatcher = signatureMatcher ?? throw new ArgumentNullException(nameof(signatureMatcher));
            _variationsProvider = variationsProvider ?? throw new ArgumentNullException(nameof(variationsProvider));
            _argumentsListContainerFactory = argumentsListContainerFactory ?? throw new ArgumentNullException(nameof(argumentsListContainerFactory));
        }

        #endregion

        #region IArgumentsGenerator methods

        /// <summary>
        /// Generates a container of all possible distinct arguments that can be passed to 
        /// a given construction, using objects from a given configuration.
        /// </summary>
        /// <param name="configuration">The wrapped configuration.</param>
        /// <param name="construction">The wrapped construction.</param>
        /// <returns>The container of resulting arguments.</returns>
        public IArgumentsListContainer GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (construction == null)
                throw new ArgumentNullException(nameof(construction));

            // First we check if we can even perform the construction. Whether there are enough
            // objects to do so. If not, we return an empty container.
            if (!CanWePerformConstruction(configuration, construction))
            {
                return _argumentsListContainerFactory.CreateContainer();
            }

            // We create an enumerable for getting all dictionaries for the combinator.
            // These are dictionaries mapping object types to the lists of objects
            // of that type. For each type we need to create an enumerable of all possible 
            // variations of the objects, having the needed count of elements.
            var dictionaryForCombinator = configuration
                    // Pull the map from the configuration
                    .AllObjectsMap
                    // Take only objects of the type that is needed for the construction
                    .Where(pair => construction.ObjectTypesToNeededCount.ContainsKey(pair.Key))
                    // Cast to the dictionary
                    .ToDictionary
                    (
                        // The key select would be simply the key of the map (the type of the object)
                        keyValue => keyValue.Key,
                        // The values should be an enumerable of all possible variations of the
                        // objects of the type. 
                        keyValue => _variationsProvider
                                // We'll ask the provider to get those variations
                                .GetVariations(keyValue.Value, construction.ObjectTypesToNeededCount[keyValue.Key])
                                // And enumerate them to the list (so we can access the object by index)
                                .Select(variation => variation.ToList())
                    );

            // Let the factory create the resulting container
            var result = _argumentsListContainerFactory.CreateContainer();

            // We let the combinator do it's job. Each dictionary yielded by the combinator
            // represents objects that will be passed to the matcher
            foreach (var dictonaryForMatcher in _combinator.Combine(dictionaryForCombinator))
            {
                // Create map wrapping the dictionary for matcher
                var map = new ConfigurationObjectsMap(dictonaryForMatcher);

                // Pull the parameters from the construction
                var parameters = construction.Construction.ConstructionParameters;

                // Let the matcher match the parameters to obtain arguments
                var arguments = _signatureMatcher.Match(parameters, map);

                // Add arguments to the container
                result.Add(arguments);
            }

            // And finally we can return the result
            return result;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns if we can perform the currently examined construction
        /// on a given configuration. In other words, check if there are enough
        /// objects to be passed as arguments to the construction.
        /// </summary>
        /// <returns>true, if the construction can be performed, false otherwise</returns>
        private bool CanWePerformConstruction(ConfigurationWrapper configuration, ConstructionWrapper construction)
        {
            // We iterate over all the pairs of object types to needed count
            foreach (var pair in construction.ObjectTypesToNeededCount)
            {
                // Pull the objects type
                var type = pair.Key;

                // Pull the needed count
                var neededCount = pair.Value;

                // Pull the map with objects
                var map = configuration.AllObjectsMap;

                // If there is no object of the type, we certainly can't perform the construction
                if (!map.ContainsKey(type))
                    return false;

                // If there are more needed arguments than available objects, 
                // then we can't perform the construction
                if (neededCount > map[type].Count)
                    return false;
            }

            // If we got here, we can perform the construction
            return true;
        }

        #endregion
    }
}