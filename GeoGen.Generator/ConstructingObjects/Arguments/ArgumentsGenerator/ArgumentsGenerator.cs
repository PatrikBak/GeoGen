using System;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities.Combinations;
using GeoGen.Utilities.Variations;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsGenerator"/>.
    /// </summary>
    internal class ArgumentsGenerator : IArgumentsGenerator
    {
        #region Private fields

        /// <summary>
        /// The signature matcher for actual conversion between construction
        /// parameters and arguments.
        /// </summary>
        private readonly IConstructionSignatureMatcher _signatureMatcher;

        /// <summary>
        /// The factory for creating an empty arguments list container
        /// that will be returned.
        /// </summary>
        private readonly IArgumentsListContainerFactory _argumentsListContainerFactory;

        /// <summary>
        /// The variations provider used to create all possible variations
        /// of objects of the same type.
        /// </summary>
        private readonly IVariationsProvider _variationsProvider;

        /// <summary>
        /// The combinator used to combine all possible options for objects of
        /// all types (provided by the variations provider) into a single dictionary
        /// </summary>
        private readonly ICombinator _combinator;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new arguments generator. The generator uses combinator and 
        /// variations provider to create all possible objects to be passed
        /// as arguments. These arguments are created by the construction signature matcher
        /// and then kept in an arguments container so that we provide only
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

        #region IArgumentsGenerator implementation

        /// <summary>
        /// Generates a container of all possible distinct arguments that can be passed to 
        /// a given construction, using objects from a given configuration.
        /// </summary>
        /// <param name="configuration">The wrapped configuration.</param>
        /// <param name="construction">The wrapped construction.</param>
        /// <returns>The container of resulting arguments.</returns>
        public IArgumentsListContainer GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction)
        {
            // First we check if we can even perform the construction. Whether there are enough
            // objects to do so. If not, we return an empty container.
            if (!CanWePerformConstruction(configuration, construction))
            {
                return _argumentsListContainerFactory.CreateContainer();
            }

            // We create an enumerable for getting all dictionaries for the combinator.
            // These dictionaries are mapping object types to the lists of objects
            // of that type. For each type we need to create an enumerable of all possible 
            // variations of the objects, having the needed count of elements.
            var dictionaryForCombinator = configuration
                    // Pull the map from the configuration
                    .WrappedConfiguration.ObjectsMap
                    // Take only objects of the type that is needed for the construction
                    .Where(pair => construction.ObjectTypesToNeededCount.ContainsKey(pair.Key))
                    // Cast to the dictionary
                    .ToDictionary
                    (
                        // The key would be simply the key of the map (the type of the object)
                        keyValue => keyValue.Key,
                        // The values should be an enumerable of all possible variations of the objects of the type. 
                        // We'll ask the provider to get those variations.
                        keyValue => _variationsProvider
                                // The number of elements can be pulled from the construction
                                .GetVariations(keyValue.Value, construction.ObjectTypesToNeededCount[keyValue.Key])
                                // And we enumerate the variations to list (so we can access the objects by index)
                                .Select(variation => variation.ToList())
                    );

            // Let the factory create the resulting container
            var result = _argumentsListContainerFactory.CreateContainer();

            // We let the combinator do it's job, which basically means, to create
            // the cartesian product of all possible variations of objects of each type.
            // Each dictionary yielded by the combinator represents objects that will be 
            // passed to the matcher. 
            foreach (var dictonaryForMatcher in _combinator.Combine(dictionaryForCombinator))
            {
                // Create a new map wrapping the dictionary for matcher
                var map = new ConfigurationObjectsMap(dictonaryForMatcher);

                // Pull the parameters from the construction
                var parameters = construction.WrappedConstruction.ConstructionParameters;

                // Let the matcher match the parameters to obtain arguments
                var arguments = _signatureMatcher.Match(parameters, map);

                // Add arguments to the container
                result.Add(arguments);
            }

            // Finally we can return the result
            return result;
        }

        /// <summary>
        /// Returns if we can perform the currently examined construction
        /// on a given configuration. In other words, checks if there are enough
        /// objects to be passed within the arguments to the construction.
        /// </summary>
        /// <returns>true, if the construction can be performed, false otherwise</returns>
        private bool CanWePerformConstruction(ConfigurationWrapper configuration, ConstructionWrapper construction)
        {
            // We iterate over all pairs of object types to needed count
            foreach (var pair in construction.ObjectTypesToNeededCount)
            {
                // Pull the objects type
                var type = pair.Key;

                // Pull the needed count
                var neededCount = pair.Value;

                // Pull the map with objects
                var map = configuration.WrappedConfiguration.ObjectsMap;

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