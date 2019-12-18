using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IGenerator"/>. It generates configurations by layers.
    /// Initially the first layer consists of the initial configuration from the input.
    /// Then in every iteration we extend all the configurations from the current layer with new 
    /// <see cref="ConstructedConfigurationObject"/>, and the <see cref="Construction"/>s/ from the input. 
    /// It guaranties generation of mutually non-symmetric configurations.
    /// </summary>
    public class Generator : IGenerator
    {
        /// <summary>
        /// Performs the generation algorithm on a given input. 
        /// </summary>
        /// <param name="input">The input for the generator.</param>
        /// <returns>The generated configurations.</returns>
        public IEnumerable<GeneratedConfiguration> Generate(GeneratorInput input)
        {
            // Prepare the instance of the class that helps us determine unique configurations
            var generationContext = new GenerationContext(input.InitialConfiguration, input.Constructions);

            // Prepare the stack holding the current enumerator
            var enumerators = new Stack<IEnumerator<GeneratedConfiguration>>();

            // The first one will be the one that uses the original configuration
            enumerators.Push(ExtendConfiguration(new GeneratedConfiguration(input.InitialConfiguration), input, generationContext));

            // Generate until there is something
            while (enumerators.Any())
            {
                // Find out if this is the final iteration
                var isThisFinalIteration = enumerators.Count == input.NumberOfIterations;

                // Take the current enumerator, without removing
                var currentEnumerator = enumerators.Peek();

                // If this is the final iteration...
                if (isThisFinalIteration)
                {
                    // Enumerate it to the end
                    while (currentEnumerator.MoveNext())
                        yield return currentEnumerator.Current;

                    // We can safely remove it
                    enumerators.Pop();

                    // We're done for now
                    continue;
                }

                // Otherwise this is not the final iteration
                // In that case move the enumerator...
                // If it's at the end...
                if (!currentEnumerator.MoveNext())
                {
                    // Then we remove it
                    enumerators.Pop();

                    // And we're done
                    continue;
                }

                // Otherwise we have generated something on a lower iteration then the final
                yield return currentEnumerator.Current;

                // Now we need to prepare the configurations that can be generated from this one
                enumerators.Push(ExtendConfiguration(currentEnumerator.Current, input, generationContext));
            }
        }

        /// <summary>
        /// Creates the enumerator that will enumerate all possible ways to extended a given configuration.
        /// </summary>
        /// <param name="currentConfiguration">The current configuration being extended.</param>
        /// <param name="input">The input for the generator.</param>
        /// <param name="context">The context of the generation that ensures that we generate distinct configurations.</param>
        /// <returns>The enumerator of generated configurations.</returns>
        private IEnumerator<GeneratedConfiguration> ExtendConfiguration(GeneratedConfiguration currentConfiguration, GeneratorInput input, GenerationContext context)
        {
            // For a given configuration we create all possible objects using the constructions from the input
            return input.Constructions.SelectMany(construction =>
            {
                // First we check if we have enough object to match the signature
                if (!construction.Signature.CanBeMatched(currentConfiguration.ObjectMap))
                    return Enumerable.Empty<ConstructedConfigurationObject>();

                // Now we're sure we can generate some objects
                // First we take all the available pairs [object type, objects]
                return currentConfiguration.ObjectMap
                        // Those are wrapped in a dictionary mapping object types to particular objects
                        // We take only those types that are required in our signature
                        .Where(keyValue => construction.Signature.ObjectTypesToNeededCount.ContainsKey(keyValue.Key))
                        // We cast each type to the IEnumerable of all possible variations of those objects
                        // having the needed number of elements. Each of these variations represents a way to
                        // use some objects of their type in our arguments
                        .Select(keyValue => keyValue.Value.Variations(construction.Signature.ObjectTypesToNeededCount[keyValue.Key]))
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
                            return construction.Signature.ObjectTypes.Select(type => typeToObjects[type][typeToIndex[type]++]).ToArray();
                        })
                        // After we have objects for the signature, we can match it to create arguments
                        .Select(construction.Signature.Match)
                        // And finally a constructed object using them
                        .Select(arguments => new ConstructedConfigurationObject(construction, arguments))
                        // We want distinct objects
                        .Distinct();
            })
            // Perform the object filter
            .Where(input.ObjectFilter)
            // Don't take the objects that would make duplicate objects in the configuration
            .Where(newObject => !currentConfiguration.ConstructedObjectsSet.Contains(newObject))
            // Add the object to the current configuration
            .Select(newObject => new GeneratedConfiguration(currentConfiguration, newObject, currentConfiguration.IterationIndex + 1))
            // Check the configuration by the helper class for exclusion
            .Where(configuration => !context.ShouldBeExcluded(configuration))
            // Apply the configuration filter
            .Where(input.ConfigurationFilter.Invoke)
            // Finally get the enumerator
            .GetEnumerator();
        }
    }


    /// <summary>
    /// The default implementation of <see cref="IGenerator"/>. It generates configurations by layers.
    /// Initially the first layer consists of the initial configuration from the input.
    /// Then in every iteration we extend all the configurations from the current layer with new 
    /// <see cref="ConstructedConfigurationObject"/>, and the <see cref="Construction"/>s/ from the input. 
    /// It guaranties generation of mutually non-symmetric configurations.
    /// </summary>
    public class Generator2 : IGenerator
    {
        /// <summary>
        /// Performs the generation algorithm on a given input. 
        /// </summary>
        /// <param name="input">The input for the generator.</param>
        /// <returns>The generated configurations.</returns>
        public IEnumerable<GeneratedConfiguration> Generate(GeneratorInput input)
        {
            #region Preparing variables

            // Prepare the configurations that are to be extended 
            var currentLayer = new List<GeneratedConfiguration>
            {
                // with the initial configuration on it.
                new GeneratedConfiguration(input.InitialConfiguration)
            };

            // Prepare the dictionary that identities created constructed objects
            // This is important for the algorithm that detects whether we haven't
            // generated a configuration symmetric to a given one before
            var objectIds = new Dictionary<ConstructedConfigurationObject, int>();

            // Prepare the set that is used to detect symmetric configurations
            // in the following way: When we generate a configuration, we generate
            // every configuration symmetric to it. Then we project constructed objects
            // of these configurations to their ids (see the line before), sort them
            // (that's why we use sorted set), and take the lexicographically minimal
            // set. It's not hard to see that two configurations are symmetric if and
            // only if the corresponding minimal sets are the same (symmetric configurations
            // yield the same set of ids sets and we're unambiguously picking one of them).
            var objectIdsSet = new HashSet<SortedSet<int>>(SortedSet<int>.CreateSetComparer());

            #endregion

            #region Algorithm

            // Execute the requested number of iterations
            for (var iterationIndex = 0; iterationIndex < input.NumberOfIterations; iterationIndex++)
            {
                #region Preparing an output enumerable

                // Prepare the enumerable that performs the algorithm that takes a configuration
                // and extends it with new objects. Each configuration will be projected into one or more generated ones
                var generationEnumerable = currentLayer.SelectMany(currentConfiguration =>
                {
                    // For a given configuration we create all possible objects
                    // using the constructions from the input
                    return input.Constructions
                            // Generate new objects from the objects of the current configuration
                            .SelectMany(construction =>
                            {
                                // First we check if we have enough object to match the signature
                                if (!construction.Signature.CanBeMatched(currentConfiguration.ObjectMap))
                                    return Enumerable.Empty<ConstructedConfigurationObject>();

                                // Now we're sure we can generate some objects
                                // First we take all the available pairs [object type, objects]
                                return currentConfiguration.ObjectMap
                                            // Those are wrapped in a dictionary mapping object types to particular objects
                                            // We take only those types that are required in our signature
                                            .Where(keyValue => construction.Signature.ObjectTypesToNeededCount.ContainsKey(keyValue.Key))
                                            // We cast each type to the IEnumerable of all possible variations of those objects
                                            // having the needed number of elements. Each of these variations represents a way to
                                            // use some objects of their type in our arguments
                                            .Select(keyValue => keyValue.Value.Variations(construction.Signature.ObjectTypesToNeededCount[keyValue.Key]))
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
                                                return construction.Signature.ObjectTypes.Select(type => typeToObjects[type][typeToIndex[type]++]).ToArray();
                                            })
                                            // After we have objects for the signature, we can match it to create arguments
                                            .Select(construction.Signature.Match)
                                            // And finally a constructed objects using them
                                            .Select(arguments => new ConstructedConfigurationObject(construction, arguments));
                            })
                            // Don't take the objects that would make duplicates in the configuration
                            .Where(newObject => !currentConfiguration.ConstructedObjectsSet.Contains(newObject))
                            // Perform the object filter
                            .Where(input.ObjectFilter)
                            // Add the object to the current configuration
                            .Select(newObject => new GeneratedConfiguration(currentConfiguration, newObject, iterationIndex + 1))
                            // Look whether we haven't generated a symmetric configuration
                            .Where(newConfiguration =>
                            {
                                // Helper function that makes sure the passed objects have ids 
                                // in the ids dictionary, projects each to this id, and wraps
                                // them in a sorted set 
                                SortedSet<int> MakeIdsSet(IEnumerable<ConstructedConfigurationObject> objects)
                                        // We simply take each object, get its id from the 
                                        // dictionary, or add a new one, based on its count
                                        => objects.Select(currentObject => objectIds.GetOrAdd(currentObject, () => objectIds.Count))
                                            // And enumerate to a sorted set
                                            .ToSortedSet();

                                // Find the ids set for the current configuration
                                var currentSet = MakeIdsSet(newConfiguration.ConstructedObjects);

                                // If we already have this configuration, we won't generate it twice
                                if (objectIdsSet.Contains(currentSet))
                                    return false;

                                // We need to find out if the current configuration if the representant
                                // of its symmetry class. Therefore we take all the symmetric mappings                            
                                var minIdsSet = currentConfiguration.LooseObjectsHolder.GetSymmetryMappings()
                                            // For every make ids set from the constructed objects 
                                            .Select(mapping => MakeIdsSet(newConfiguration.ConstructedObjects
                                                // that are remapped using this mapping
                                                .Select(constructedObject => (ConstructedConfigurationObject)constructedObject.Remap(mapping))))
                                            // Take the lexicographically minimal ids set
                                            .Min((a1, a2) => a1.CompareToLexicographically(a2));

                                // If this configuration is not the representant of its symmetry class,
                                // then we say it is not correct
                                if (!minIdsSet.SequenceEqual(currentSet))
                                    return false;

                                // Otherwise we mark it as a generated one
                                objectIdsSet.Add(currentSet);

                                // And return that it's fine
                                return true;
                            })
                            // Perform the configuration filter
                            .Where(input.ConfigurationFilter);
                });

                #endregion

                #region Enumerating 

                // Prepare a new layer
                var newLayer = new List<GeneratedConfiguration>();

                // Enumerate the output enumerable
                foreach (var configuration in generationEnumerable)
                {
                    // Add the output to the new layer
                    newLayer.Add(configuration);

                    // Return it
                    yield return configuration;
                }

                // Set the current layer to the new one
                currentLayer = newLayer;

                #endregion
            }

            #endregion
        }
    }
}