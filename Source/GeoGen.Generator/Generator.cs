using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IGenerator"/>. The generator
    /// uses an <see cref="IConfigurationsManager"/> for performing the gradual
    /// constructing, an <see cref="IObjectsConstructor"/> for constructing
    /// new output for each configuration, and an <see cref="ITheoremsAnalyzer"/>
    /// for performing the actual analysis of theorems.
    /// </summary>
    internal class Generator : IGenerator
    {
        #region Private fields

        /// <summary>
        /// The manager used for the gradual generation of configurations.
        /// </summary>
        private readonly IConfigurationsManager _configurationsManager;

        /// <summary>
        /// The constructor of new <see cref="ConstructorOutput"/>s for each configuration.
        /// </summary>
        private readonly IObjectsConstructor _objectsConstructor;

        /// <summary>
        /// The analyzer that performs the actual theorems finding.
        /// </summary>
        private readonly ITheoremsAnalyzer _analyzer;

        /// <summary>
        /// The container of all the generated objects.
        /// </summary>
        private IConfigurationObjectsContainer _container;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="manager">The manager of generated configurations.</param>
        /// <param name="constructor">The constructor of new objects to configurations.</param>
        /// <param name="container">The container of all the generated objects.</param>
        /// <param name="analyzer">The analyzer of generated configurations.</param>
        /// <param name="iterations">The number of iterations to be performed.</param>
        public Generator(IConfigurationsManager manager, IObjectsConstructor constructor, IConfigurationObjectsContainer container, ITheoremsAnalyzer analyzer)
        {
            _configurationsManager = manager ?? throw new ArgumentNullException(nameof(manager));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _objectsConstructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region IGenerator methods

        /// <summary>
        /// Starts the generation process and lazily returns the output.
        /// </summary>
        /// <returns>The generator output enumerable.</returns>
        public IEnumerable<GeneratorOutput> Generate(int numberOfIterations)
        {
            // Add initial objects

            // Executed the needed number of iterations
            return Enumerable.Range(0, numberOfIterations)
                    // In each create many configurations
                    .SelectMany(iterationIndex =>
                    {
                        // From the container
                        var outputForNewLayer = _configurationsManager
                                // Take the current layer
                                .CurrentLayer
                                // Construct outputs from each of them
                                .SelectMany(configuration => _objectsConstructor.GenerateOutput(configuration))
                                // Make sure the objects are identified 
                                .Select(output =>
                                {
                                    // Go through all the constructed objects
                                    for (var i = 0; i < output.ConstructedObjects.Count; i++)
                                    {
                                        // Add the current one
                                        _container.Add(output.ConstructedObjects[i], out var equalObject);

                                        // If there is an equal object...
                                        if (equalObject != null)
                                        {
                                            // Reset the position in the output
                                            // This step makes sure that equal objects won't be used with
                                            // more than one instance. The objects that we will 'rewrite' 
                                            // shouldn't be used elsewhere and thus will be eventually 
                                            // eaten by the garbage collector
                                            output.ConstructedObjects[i] = (ConstructedConfigurationObject) equalObject;
                                        }
                                    }

                                    // Return the output
                                    return output;
                                });

                        // Return the enumerable for creating the new layer
                        return _configurationsManager.AddLayer(outputForNewLayer);
                    })
                    .Select(configurationWrapper =>
                    {
                        // Unwrap configuration
                        var unwrappedConfiguration = configurationWrapper.WrappedConfiguration;

                        // Call the analyzer
                        var theorems = _analyzer.Analyze(unwrappedConfiguration);

                        // Return the output
                        return new GeneratorOutput
                        {
                            Configuration = unwrappedConfiguration,
                            Theorems = theorems
                        };
                    });
        }

        #endregion
    }
}