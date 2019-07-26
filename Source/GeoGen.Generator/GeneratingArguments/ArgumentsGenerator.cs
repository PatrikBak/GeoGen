using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IArgumentsGenerator"/> that uses 
    /// <see cref="IArgumentsContainerFactory"/> for generating empty containers
    /// of <see cref="Arguments"/> that recognize equal arguments, so that we won't
    /// return duplicate result. 
    /// </summary>
    public class ArgumentsGenerator : IArgumentsGenerator
    {
        #region Dependencies

        /// <summary>
        /// The factory for creating an empty arguments container that rules out equal arguments.
        /// </summary>
        private readonly IArgumentsContainerFactory _argumentsContainerFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentsGenerator"/> class.
        /// </summary>
        /// <param name="argumentsContainerFactory">The factory for creating an empty arguments container that rules out equal arguments.</param>
        public ArgumentsGenerator(IArgumentsContainerFactory argumentsContainerFactory)
        {
            _argumentsContainerFactory = argumentsContainerFactory ?? throw new ArgumentNullException(nameof(argumentsContainerFactory));
        }

        #endregion

        #region IArgumentsGenerator implementation

        /// <summary>
        /// Generates all possible arguments, that match a given signature, 
        /// using the configurations objects from a given configuration objects map.
        /// </summary>
        /// <param name="availableObjects">The configuration objects map of the objects that can be used in generated arguments.</param>
        /// <param name="construction">The signature that should be matched.</param>
        /// <returns>The generated arguments enumerable.</returns>
        public IEnumerable<Arguments> GenerateArguments(ConfigurationObjectsMap availableObjects, Signature signature)
        {
            // First we check if we have even enough object to match the signature
            if (!signature.CanBeMatched(availableObjects))
                yield break;

            // Now we're sure we can generate some arguments
            // We're going to prepare the enumerable that generates all possible arguments
            // First we take all the available objects
            var argumentsGenerationEnumerable = availableObjects
                    // Those are wrapped in a dictionary mapping object types to particular objects
                    // We take only those types that are required in our signature
                    .Where(keyValue => signature.ObjectTypesToNeededCount.ContainsKey(keyValue.Key))
                    // We cast each type to the IEnumerable of all possible variations of those objects
                    // having the needed number of elements. Each of these variations represents a way to
                    // use some objects of their type in our arguments
                    .Select(keyValue => keyValue.Value.Variations(signature.ObjectTypesToNeededCount[keyValue.Key]))
                    // We need to combine all these options for particular object types in every possible way
                    // For example, if we need 2 points and 2 lines, and we have 4 pair of points and 5 pairs of lines
                    // then there are 20 ways of combining these pairs. The result of this call will be an enumerable
                    // where each element is an array of options (each array representing an option for some particular type)
                    .Combine()
                    // We need to create a single array representing an input for the signature.Match method
                    .Select(arrays =>
                    {
                        // Create a helper dictionary mapping types to an array of objects of these types
                        var typeToObjects = arrays.ToDictionary(objects => objects[0].ObjectType, objects => objects);

                        // Create a helper dictionary mapping types to the current index (pointer) on the next object
                        // of this type that should be returned
                        var typeToIndex = arrays.ToDictionary(objects => objects[0].ObjectType, objects => 0);

                        // We go through the requested types and for each we take the object of the given type
                        // and at the same moment increase the index so that it points out to the next object of this type
                        return signature.ObjectTypes.Select(type => typeToObjects[type][typeToIndex[type]++]).ToArray();
                    })
                    // After we have the map, we'll use the signature's method to create the corresponding arguments 
                    .Select(signature.Match);

            // Let the factory create an empty container where we will be adding the generated arguments
            var generatedArguments = _argumentsContainerFactory.CreateContainer();

            // Enumerate the arguments generation enumerable
            foreach (var arguments in argumentsGenerationEnumerable)
            {
                // Add the generated arguments to the container
                // It will find if we already have these
                generatedArguments.TryAdd(arguments, out var equalArguments);

                // Return them if there are no equal arguments
                if (equalArguments == null)
                    yield return arguments;
            }
        }

        #endregion
    }
}