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
        /// Generates all possible arguments, that match a given construction, 
        /// using the configurations objects from a given configuration objects map.
        /// </summary>
        /// <param name="availableObjects">The configuration objects map of the objects that can be used in generated arguments.</param>
        /// <param name="construction">The construction which signature should be matched.</param>
        /// <returns>The generated arguments enumerable.</returns>
        public IEnumerable<Arguments> GenerateArguments(ConfigurationObjectsMap availableObjects, Construction construction)
        {
            // First we check if we can even perform the construction. In other words, whether 
            // there is enough objects to do so. If not, we simply break an empty container.
            if (!CanWePerformConstruction(availableObjects, construction))
                yield break;

            // Now we're sure we can generate some arguments
            // We're going to prepare the enumerable that generates all possible arguments
            // First we take all the available objects
            var argumentsGenerationEnumerable = availableObjects
                    // Those are wrapped in a dictionary mapping object types to particular objects
                    // We take only those types that are required in our construction
                    .Where(keyValue => construction.ObjectTypesToNeededCount.ContainsKey(keyValue.Key))
                    // We cast each type to the IEnumerable of all possible variations of those objects
                    // having the needed number of elements. Each of these variations represents a way to
                    // use some objects of their type in our arguments
                    .Select(keyValue => keyValue.Value.Variations(construction.ObjectTypesToNeededCount[keyValue.Key]))
                    // We need to combine all these options for particular object types in every possible way
                    // For example, if we need 2 points and 2 lines, and we have 4 pair of points and 5 pairs of lines
                    // then there are 20 ways of combining these pairs. The result of this call will be an enumerable
                    // where each element is an array of options (each array representing an option for some particular type)
                    .Combine()
                    // We wrap each option into an objects map. Before doing so to flatten the arrays, which basically
                    // means putting more arrays (of objects of different types) into a single one
                    .Select(arrays => new ConfigurationObjectsMap(arrays.Flatten()))
                    // After we have the map, we'll use the private method to create the corresponding arguments 
                    .Select(objectsToUse => Match(objectsToUse, construction.ConstructionParameters));

            // Let the factory create an empty container where we will be adding the generated arguments
            var generatedArguments = _argumentsContainerFactory.CreateContainer();

            // Enumerate the arguments generation enumerable
            foreach (var arguments in argumentsGenerationEnumerable)
            {
                // Add the generated arguments to the container
                // It will find if we already have these
                generatedArguments.Add(arguments, out var equalArguments);

                // Return them if there are no equal arguments
                if (equalArguments == null)
                    yield return arguments;
            }
        }

        /// <summary>
        /// Returns if we can perform the currently examined construction from given objects.
        /// </summary>
        /// <param name="availableObjects">The available objects that we can use.</param>
        /// <param name="construction">The construction that we're examining.</param>
        /// <returns>true, if the construction can be performed, false otherwise</returns>
        private bool CanWePerformConstruction(ConfigurationObjectsMap availableObjects, Construction construction)
        {
            // Let's have a look at each pair of [ObjectType, NeededCount] to find out
            // if we have enough objects from this type
            foreach (var pair in construction.ObjectTypesToNeededCount)
            {
                // Pull the objects type
                var type = pair.Key;

                // Pull the needed count
                var neededCount = pair.Value;

                // If there is no object of the type, we certainly can't perform the construction
                if (!availableObjects.ContainsKey(type))
                    return false;

                // If there are more needed arguments than available objects, 
                // then we can't perform the construction either
                if (neededCount > availableObjects[type].Count)
                    return false;
            }

            // If we got here, we can perform the construction
            return true;
        }

        /// <summary>
        /// Constructs construction arguments that match the given construction 
        /// parameters. The objects that are actually passed to the arguments 
        /// are given in a configuration objects map.
        /// </summary>
        /// <param name="parameters">The parameters to be matched.</param>
        /// <param name="objectsMap">The configuration objects used in the created arguments.</param>
        /// <returns>The created arguments matching the parameters.</returns>
        private Arguments Match(ConfigurationObjectsMap objectsMap, IReadOnlyList<ConstructionParameter> parameters)
        {
            // Create a dictionary mapping object types to the current index of the object of that type
            // that is ready to be passed to the next object argument. Initially, these indices are 0.
            var indices = objectsMap.ToDictionary(keyValue => keyValue.Key, keyValue => 0);

            // Local function that creates an argument matching a given parameter
            ConstructionArgument CreateArgument(ConstructionParameter parameter)
            {
                // If the parameter is an object parameter...
                if (parameter is ObjectConstructionParameter objectParameter)
                {
                    // Then we simply ask for the next object of the expected type
                    // and increase the index for the next expected object of this type
                    var nextObject = objectsMap[objectParameter.ExpectedType][indices[objectParameter.ExpectedType]++];

                    // And return the object argument wrapping this object
                    return new ObjectConstructionArgument(nextObject);
                }

                // Otherwise we have a set construction parameter
                var setParameter = (SetConstructionParameter) parameter;

                // Create arguments list that we're going to fill
                var arguments = new List<ConstructionArgument>();

                // For the expected number of times...
                GeneralUtilities.ExecuteNTimes(setParameter.NumberOfParameters, () =>
                {
                    // Recursively call this function to obtain a new argument
                    var newArgument = CreateArgument(setParameter.TypeOfParameters);

                    // And update the arguments list
                    arguments.Add(newArgument);
                });

                // Finally return the set construction argument wrapping the filled list
                return new SetConstructionArgument(arguments);
            }

            // Execute the create arguments function for particular parameters and wrap the result into Arguments
            return new Arguments(parameters.Select(CreateArgument).ToList());
        }

        #endregion
    }
}