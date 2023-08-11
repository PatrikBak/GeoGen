using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.ConfigurationGenerator
{
    /// <summary>
    /// The default implementation of <see cref="IConfigurationGenerator"/>. It generates configurations by layers.
    /// Initially the first layer consists of the initial configuration from the input.
    /// Then in every iteration we extend all the configurations from the current layer with new 
    /// <see cref="ConstructedConfigurationObject"/>, and the <see cref="Construction"/>s/ from the input. 
    /// It guaranties generation of mutually non-symmetric configurations.
    /// </summary>
    public class ConfigurationGenerator : IConfigurationGenerator
    {
        #region Dependencies

        /// <summary>
        /// The factory for creating <see cref="IConfigurationFilter"/> for the whole generation process.
        /// </summary>
        private readonly IConfigurationFilterFactory _factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGenerator"/> class.
        /// </summary>
        /// <param name="factory">The factory for creating <see cref="IConfigurationFilter"/> for the whole generation process.</param>
        public ConfigurationGenerator(IConfigurationFilterFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region IConfigurationGenerator implementation

        /// <inheritdoc/>
        public IEnumerable<GeneratedConfiguration> Generate(ConfigurationGeneratorInput input)
        {
            // Let the factory create the filter to be used
            var configurationFilter = _factory.Create(input);

            // Prepare the stack holding the current enumerator
            var enumerators = new Stack<IEnumerator<GeneratedConfiguration>>();

            // The first one will be the one that uses the original configuration
            enumerators.Push(ExtendConfiguration(new GeneratedConfiguration(input.InitialConfiguration), input, configurationFilter));

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
                enumerators.Push(ExtendConfiguration(currentEnumerator.Current, input, configurationFilter));
            }
        }

        /// <summary>
        /// Creates the enumerator that will enumerate all possible ways to extended a given configuration.
        /// </summary>
        /// <param name="currentConfiguration">The current configuration being extended.</param>
        /// <param name="input">The input for the generator.</param>
        /// <param name="filter">The filter of generated configurations that should ensure that we generate distinct configurations.</param>
        /// <returns>The enumerator of generated configurations.</returns>
        private static IEnumerator<GeneratedConfiguration> ExtendConfiguration(GeneratedConfiguration currentConfiguration, ConfigurationGeneratorInput input, IConfigurationFilter filter)
        {
            // For a given configuration we create all possible objects using the constructions from the input
            return input.Constructions.SelectMany(construction =>
            {
                // First we check if we have enough object to match the signature
                if (!construction.Signature.CanBeMatched(currentConfiguration.ObjectMap))
                    return Enumerable.Empty<ConstructedConfigurationObject>();

                // Now we check whether adding an object of the current type wouldn't exceed the maximal number
                // Find the number of added objects of this type as all objects of this type 
                var numberOfAddedObjectsOfTheCurrentType = currentConfiguration.ObjectMap.GetObjectsForKeys(construction.OutputType).Count()
                    // Minus initial objects of this type
                    - input.InitialConfiguration.ObjectMap.GetObjectsForKeys(construction.OutputType).Count();

                // If adding of a new object would exceed the maximal requested count, we're done
                if (numberOfAddedObjectsOfTheCurrentType + 1 > input.MaximalNumbersOfObjectsToAdd[construction.OutputType])
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
            // Don't take the objects that would make duplicate objects in the configuration
            .Where(newObject => !currentConfiguration.ConstructedObjectsSet.Contains(newObject))
            // Add the object to the current configuration
            .Select(newObject => new GeneratedConfiguration(currentConfiguration, newObject, currentConfiguration.IterationIndex + 1))
            // Check the configuration by the filter
            .Where(configuration => !filter.ShouldBeExcluded(configuration))
            // Apply the custom configuration filter
            .Where(input.ConfigurationFilter.Invoke)
            // Finally get the enumerator
            .GetEnumerator();
        }

        #endregion
    }
}