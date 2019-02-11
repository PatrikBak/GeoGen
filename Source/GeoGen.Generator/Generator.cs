using GeoGen.Core;
using GeoGen.GeometryRegistrar;
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
    /// is <see cref="ConfigurationObject"/>, to recognize equal objects. Then we validate each created 
    /// configuration formally (no equal objects, not already generated) and geometrically (constructible
    /// and no duplicated) and return only valid ones. The valid configurations represent the next layer. 
    /// The generation process is lazy. Once it has started, we can't run it again with the same instance of the class.
    /// </summary>
    public class Generator : IGenerator
    {
        #region Dependencies

        /// <summary>
        /// The factory for creating a container for configuration objects that recognizes equal ones.
        /// </summary>
        private IConfigurationObjectsContainerFactory _objectsContainerFactory;

        /// <summary>
        /// The factory for creating a container for generated configurations that recognizes equal ones.
        /// </summary>
        private IConfigurationsContainerFactory _configurationsContainerFactory;

        /// <summary>
        /// The generator of arguments that are passed to newly created constructed objects.
        /// </summary>
        private readonly IArgumentsGenerator _argumentsGenerator;

        /// <summary>
        /// The registrar that perform the actual geometrical construction of configurations.
        /// </summary>
        private readonly IGeometryRegistrar _geometryRegistrar;

        /// <summary>
        /// The tracer of geometrically equal objects determined by the registrar.
        /// </summary>
        private readonly IEqualObjectsTracer _equalObjectsTracer;

        /// <summary>
        /// The tracer of geometrically inconstructible objects determined by the registrar.
        /// </summary>
        private readonly IInconstructibleObjectsTracer _inconstructibleObjectsTracer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="objectsContainerFactory">The factory for creating a container for configuration objects that recognizes equal ones.</param>
        /// <param name="configurationsContainerFactory">The factory for creating a container for generated configurations that recognizes equal ones.</param>
        /// <param name="argumentsGenerator">The generator of arguments that are passed to newly created constructed objects.</param>
        /// <param name="geometryRegistrar">The registrar that perform the actual geometrical construction of configurations.</param>
        /// <param name="equalObjectsTracer">The tracer of geometrically equal objects determined by the registrar.</param>
        /// <param name="inconstructibleObjectsTracer">The tracer of geometrically inconstructible objects determined by the registrar.</param>
        public Generator(IConfigurationObjectsContainerFactory objectsContainerFactory,
                         IConfigurationsContainerFactory configurationsContainerFactory,
                         IArgumentsGenerator argumentsGenerator,
                         IGeometryRegistrar geometryRegistrar,
                         IEqualObjectsTracer equalObjectsTracer = null,
                         IInconstructibleObjectsTracer inconstructibleObjectsTracer = null)
        {
            _objectsContainerFactory = objectsContainerFactory ?? throw new ArgumentNullException(nameof(objectsContainerFactory));
            _configurationsContainerFactory = configurationsContainerFactory ?? throw new ArgumentNullException(nameof(configurationsContainerFactory));
            _argumentsGenerator = argumentsGenerator ?? throw new ArgumentNullException(nameof(argumentsGenerator));
            _geometryRegistrar = geometryRegistrar ?? throw new ArgumentNullException(nameof(geometryRegistrar));
            _equalObjectsTracer = equalObjectsTracer;
            _inconstructibleObjectsTracer = inconstructibleObjectsTracer;
        }

        #endregion

        #region IGenerator implementation

        /// <summary>
        /// Starts the generation process and lazily returns the output. 
        /// </summary>
        /// <param name="input">The input for the generator.</param>
        /// <returns>A lazy enumerable of generator outputs.</returns>
        public IEnumerable<GeneratorOutput> Generate(GeneratorInput input)
        {
            #region Prepare variables

            // The configurations that are to be extended
            var currentLayer = new List<GeneratedConfiguration>();

            // The id prepared to be set to the next generated configuration object.
            var nextConfigurationObjectId = 0;

            // The id prepared to be set to the next generated configuration.
            var nextConfigurationId = 0;

            // The set containing ids of inconstructible objects. 
            var inconstructibleObjectsIds = new HashSet<int>();

            // The container for configuration objects
            var objectsContainer = _objectsContainerFactory.CreateContainer();

            // The container for configurations
            var configurationsContainer = _configurationsContainerFactory.CreateContainer();

            #endregion

            #region Initialize constructions

            // Make sure the constructions have distinct names
            if (input.Constructions.Select(construction => construction.Name).Count() != input.Constructions.Count)
                throw new InitializationException("The constructions are supposed to have mutually distinct names");

            // Map all the construction to their names 
            // We have to include the construction used to construct initial objects as well 
            // Take all the constructions of the initial objects
            input.InitialConfiguration.ConstructedObjects.Select(obj => obj.Construction)
                // Merge with the input constructions
                .Concat(input.Constructions)
                // Group them according to their name
                .GroupBy(construction => construction.Name)
                // Identify each group with the same id
                .ForEach((group, index) => group.ForEach(construction => construction.Id = index));

            #endregion

            #region Initialize the initial objects

            // For each object...
            input.InitialConfiguration.ObjectsMap.AllObjects.ForEach(obj =>
            {
                // Make sure it is identified
                obj.Id = nextConfigurationObjectId++;

                // And added to the container
                objectsContainer.TryAdd(obj, out var equalObject);

                // Make sure there are no duplicates...
                if (equalObject != null)
                    throw new InitializationException($"The initial configuration contains duplicate objects.");
            });

            #endregion

            #region Handle the first configuration

            // Create the initial generated configuration from the passed initial one
            var firstConfiguration = new GeneratedConfiguration(input.InitialConfiguration, nextConfigurationId++);

            // Add it to the container
            configurationsContainer.Add(firstConfiguration);

            // Add it to the first layer
            currentLayer.Add(firstConfiguration);

            // Register it to the system
            var firstConfigurationRegistrationResult = _geometryRegistrar.Register(firstConfiguration);

            // Make sure it's been examined correctly
            if (!firstConfigurationRegistrationResult.SuccessfullyExamined)
                throw new InitializationException("Drawing of the initial configuration failed.");

            // Make sure it is constructible
            if (firstConfigurationRegistrationResult.InconstructibleObject != null)
                throw new InitializationException($"The initial configuration contains an inconstructible object.");

            // Make sure there are no duplicates...
            if (firstConfigurationRegistrationResult.Duplicates != (null, null))
                throw new InitializationException($"The initial configuration contains duplicate objects.");

            #endregion

            #region Algorithm

            // Execute the requested number of iterations
            for (var iterationIndex = 0; iterationIndex < input.NumberOfIterations; iterationIndex++)
            {
                // In each iteration we initialize a list for new configurations that will be extended
                // in the next iteration. We're gonna fill it with our the configurations generated
                // from the current layer
                var generatedConfigurations = new List<GeneratedConfiguration>();

                // Prepare the enumerable that performs the algorithm that takes a configuration
                // and extends it with new objects. Each configuration will be projected into one or more generator outputs
                var generatedOutputEnumerable = currentLayer.SelectMany(currentConfiguration =>
                {
                    // For a given configuration we create all possible objects that are use our constructions
                    return input.Constructions.SelectMany(construction =>
                        {
                            // For a given construction first generate all possible arguments
                            var generatedArguments = _argumentsGenerator.GenerateArguments(currentConfiguration.ObjectsMap, construction.Signature);

                            // Cast each arguments to a new constructed object
                            return generatedArguments.Select(arguments => new ConstructedConfigurationObject(construction, arguments, nextConfigurationObjectId++));
                        })
                        // Make sure each constructed object are added to the container so we can recognize equal ones
                        .Select(newObject =>
                        {
                            // Add the new object to the container. It will found out if there 
                            // already is an equal version of it inside
                            objectsContainer.TryAdd(newObject, out var equalObject);

                            // If there is an equal object already in the container
                            if (equalObject != null)
                            {
                                // Then we'll re-assign the value to be returned further
                                // This also step makes sure that equal objects won't be 
                                // used with more than one instance. The objects that we 
                                // will re-assign shouldn't be used elsewhere and thus 
                                // will be eventually eaten by the garbage collector. 
                                newObject = (ConstructedConfigurationObject) equalObject;
                            }

                            // Return the new object
                            return newObject;
                        })
                        // For each new object creates a new generated configuration
                        .Select(newObject => new GeneratedConfiguration(currentConfiguration, newObject, nextConfigurationId++))
                        // Take only valid ones
                        .Where(configuration =>
                        {
                            #region Object ids based validation

                            // Let's find the object ids
                            var objectIds = configuration.ObjectsMap.AllObjects.Select(obj => obj.Id).ToSet();

                            // Check if there is no duplicate, i.e. the number of ids is equal to the number of objects
                            if (configuration.ObjectsMap.AllObjects.Count != objectIds.Count)
                                return false;

                            // Check if there is any id that has been marked as inconstructible
                            if (objectIds.Any(inconstructibleObjectsIds.Contains))
                                return false;

                            // We're sure the configuration is formally correct (no duplicates)
                            // and that can't be excluded based on the previous data (inconstructible objects)

                            #endregion

                            #region Equal configurations validation

                            // Add the configuration to the container
                            configurationsContainer.TryAdd(configuration, out var equalConfiguration);

                            // If there already is an equal version of it, this one is invalid
                            if (equalConfiguration != null)
                                return false;

                            // We're sure the configuration is new, i.e. we haven't seen an equal version of it yet

                            #endregion

                            // In this case the configuration is correct and ready to be drawn
                            return true;
                        })
                        // Register them
                        .Select(configuration => (configuration, registrationResult: _geometryRegistrar.Register(configuration)))
                        // Take only geometrically valid ones
                        .Where(tuple =>
                        {
                            // Deconstruct 
                            var (configuration, registrationResult) = tuple;

                            // If the registration didn't work out, then we have to say the configuration is invalid
                            if (!registrationResult.SuccessfullyExamined)
                                return false;

                            // Find out if there is an inconstructible object
                            var anyInconstructibleObject = registrationResult.InconstructibleObject != null;

                            // Find out if there are any duplicates
                            var anyDuplicates = registrationResult.Duplicates != (null, null);

                            // If there is an inconstructible object
                            if (anyInconstructibleObject)
                            {
                                // Then we want to remember its id
                                inconstructibleObjectsIds.Add(registrationResult.InconstructibleObject.Id);

                                // And trace it
                                _inconstructibleObjectsTracer?.TraceInconstructibleObject(registrationResult.InconstructibleObject);
                            }

                            // If there are duplicates...
                            if (anyDuplicates)
                            {
                                // We deconstruct the older and newer objects
                                var (olderObject, newerObject) = registrationResult.Duplicates;

                                // And trace them
                                _equalObjectsTracer?.TraceEqualObjects(olderObject, newerObject);
                            }

                            // If there are any invalid objects or duplicates, then it's incorrect
                            if (anyInconstructibleObject || anyDuplicates)
                                return false;

                            // After all the validations passed, return that it's valid
                            return true;
                        })
                        // Finally construct the output for each of them
                        .Select(tuple =>
                        {
                            // Construct the output
                            return new GeneratorOutput
                            {
                                // Set the configuration
                                Configuration = tuple.configuration,

                                // Set the manager
                                Manager = tuple.registrationResult.Manager
                            };
                        });
                });

                // Perform the generation by enumerating the enumerable 
                // and lazily return the output. This generation will
                // gradually fill the 'generatedConfigurations' list
                foreach (var output in generatedOutputEnumerable)
                    yield return output;

                // At this point the next layer is prepared
                // We can set our generated configurations to be the next layer
                currentLayer = new List<GeneratedConfiguration>(generatedConfigurations);
            }

            #endregion
        }

        #endregion
    }
}