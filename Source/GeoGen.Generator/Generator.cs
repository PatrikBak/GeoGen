using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IGenerator"/>. It generates configurations by layers.
    /// Initially the first layer consists of the initial configuration from the input.
    /// Then in every iteration we extend all the configurations from the current layer with new 
    /// <see cref="ConstructedConfigurationObject"/>, that are created using <see cref="IArgumentsGenerator"/>,
    /// and the <see cref="Construction"/>s/ from the input. We use <see cref="IContainer{T}"/>, where 'T' 
    /// is <see cref="ConfigurationObject"/>, to recognize equal objects.
    /// </summary>
    public class Generator : IGenerator
    {
        #region Dependencies

        /// <summary>
        /// The factory for creating a container for configuration objects that recognizes equal ones.
        /// </summary>
        private readonly IConfigurationObjectsContainerFactory _objectsContainerFactory;

        /// <summary>
        /// The factory for creating a container for generated configurations that recognizes equal ones.
        /// </summary>
        private readonly IConfigurationsContainerFactory _configurationsContainerFactory;

        /// <summary>
        /// The generator of arguments that are passed to newly created constructed objects.
        /// </summary>
        private readonly IArgumentsGenerator _generator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="objectsContainerFactory">The factory for creating a container for configuration objects that recognizes equal ones.</param>
        /// <param name="configurationsContainerFactory">The factory for creating a container for generated configurations that recognizes equal ones.</param>
        /// <param name="generator">The generator of arguments that are passed to newly created constructed objects.</param>
        public Generator(IConfigurationObjectsContainerFactory objectsContainerFactory, IConfigurationsContainerFactory configurationsContainerFactory, IArgumentsGenerator generator)
        {
            _objectsContainerFactory = objectsContainerFactory ?? throw new ArgumentNullException(nameof(objectsContainerFactory));
            _configurationsContainerFactory = configurationsContainerFactory ?? throw new ArgumentNullException(nameof(configurationsContainerFactory));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        #endregion

        #region IGenerator implementation

        /// <summary>
        /// Performs the generation algorithm.
        /// </summary>
        /// <param name="input">The input for the algorithm.</param>
        /// <param name="objectFilter">The filter applied to generated constructed objects.</param>
        /// <param name="configurationFilter">The filter applied to generated configuration.</param>
        /// <returns>The generated configurations.</returns>
        public IEnumerable<GeneratedConfiguration> Generate(GeneratorInput input, Predicate<ConstructedConfigurationObject> objectFilter, Predicate<GeneratedConfiguration> configurationFilter)
        {
            #region Prepare variables

            // The configurations that are to be extended
            var currentLayer = new List<GeneratedConfiguration>();

            // The container for configuration objects
            var objectsContainer = _objectsContainerFactory.CreateContainer();

            // The container for configurations
            var configurationsContainer = _configurationsContainerFactory.CreateContainer();

            #endregion

            #region Prepare the initial configuration

            // Add all the initial objects
            input.InitialConfiguration.AllObjects.ForEach(configurtionObject => objectsContainer.TryAdd(configurtionObject, out var _));

            // Create the initial generated configuration from the passed initial one
            var firstConfiguration = new GeneratedConfiguration(input.InitialConfiguration);

            // Add it to the container
            configurationsContainer.Add(firstConfiguration);

            // Add it to the first layer
            currentLayer.Add(firstConfiguration);

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
                    // For a given configuration we create all possible objects that are use our constructions
                    //
                    // TODO: Consider not using every construction. Over-using constructions is not pretty
                    //       Maybe another filter coming as a parameter?
                    // 
                    return input.Constructions.SelectMany(construction =>
                    {
                        // For a given construction first generate all possible arguments
                        return _generator.GenerateArguments(currentConfiguration.ObjectsMap, construction.Signature)
                            // Create a constructed objects from each
                            .Select(arguments => new ConstructedConfigurationObject(construction, arguments));
                    })
                    // Make sure each constructed object are added to the container so we can recognize equal ones
                    .Select(newObject =>
                    {
                        // Add the new object to the container. It will found out if there 
                        // already is an equal version of it inside
                        objectsContainer.TryAdd(newObject, out var equalObject);

                        // If there is an equal object already in the container
                        if (equalObject != default)
                        {
                            // Then we'll re-assign the value to be returned further
                            // This also step makes sure that equal objects won't be 
                            // used with more than one instance. The objects that we 
                            // will re-assign shouldn't be used elsewhere and thus 
                            // will be eventually eaten by the garbage collector. 
                            newObject = (ConstructedConfigurationObject)equalObject;
                        }

                        // Return the new object
                        return newObject;
                    })
                    // Perform the passed validation of the generated object
                    .Where(objectFilter.Invoke)
                    // For each valid object create a new generated configuration
                    .Select(newObject => new GeneratedConfiguration(currentConfiguration, newObject, iterationIndex + 1))
                    // Make sure it has no duplicates and there is no configuration equal to it yet
                    .Where(configuration =>
                    {
                        #region Duplicate objects validation

                        // Make sure the last object is not equal to any previous ones
                        // TODO: Replace with Indexers once they're out
                        if (configuration.ConstructedObjects.Reverse().Skip(1).Any(configurationObject => configurationObject == configuration.LastConstructedObject))
                            return false;

                        #endregion

                        #region Equal configurations validation

                        // Add the configuration to the container
                        configurationsContainer.TryAdd(configuration, out var equalConfiguration);

                        // If there already is an equal version of it, this one is invalid
                        if (equalConfiguration != default)
                            return false;

                        // We're sure the configuration is new, i.e. we haven't seen an equal version of it yet

                        #endregion

                        // In this case the configuration is correct
                        return true;
                    })
                    // Perform the passed configuration validation 
                    .Where(configurationFilter.Invoke);
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

        #endregion
    }
}