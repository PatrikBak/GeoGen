using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer;
using GeoGen.Core.Generator;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IGenerator"/>. The generator
    /// uses an <see cref="IConfigurationsManager"/> for performing the gradual
    /// constructing, an <see cref="IObjectsConstructor"/> for constructing
    /// new output for each configuration, and an <see cref="IGradualAnalyzer"/>
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
        /// The gradual analyzer that performs the actual theorems finding.
        /// </summary>
        private readonly IGradualAnalyzer _gradualAnalyzer;

        /// <summary>
        /// The maximal number of iterations that are supposed to be performed.
        /// </summary>
        private readonly int _maximalNumberOfIterations;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configurationsManager">The manager of generated configurations.</param>
        /// <param name="objectsConstructor">The constructor of new objects to configurations.</param>
        /// <param name="gradualAnalyzer">The analyzer of generated configurations.</param>
        /// <param name="maximalNumberOfIterations">The maximal number of iterations to be performed.</param>
        public Generator
        (
            IConfigurationsManager configurationsManager,
            IObjectsConstructor objectsConstructor,
            IGradualAnalyzer gradualAnalyzer,
            int maximalNumberOfIterations
        )
        {
            _configurationsManager = configurationsManager ?? throw new ArgumentNullException(nameof(configurationsManager));
            _gradualAnalyzer = gradualAnalyzer ?? throw new ArgumentNullException(nameof(gradualAnalyzer));
            _objectsConstructor = objectsConstructor ?? throw new ArgumentNullException(nameof(objectsConstructor));
            _maximalNumberOfIterations = maximalNumberOfIterations;
        }

        #endregion

        #region IGenerator methods

        /// <summary>
        /// Starts the generation process and lazily returns the output.
        /// </summary>
        /// <returns>The generator output enumerable.</returns>
        public IEnumerable<GeneratorOutput> Generate()
        {
            return Enumerable.Range(0, _maximalNumberOfIterations).SelectMany(iterationIndex =>
                    {
                        // From the container
                        var outputForNewLayer = _configurationsManager
                                // Take the current layer
                                .CurrentLayer
                                // Construct outputs from each of them
                                .SelectMany(configuration => _objectsConstructor.GenerateOutput(configuration));

                        // Return the enumerable for creating the new layer
                        return _configurationsManager.AddLayer(outputForNewLayer);
                    })
                    .Select(configurationWrapper =>
                    {
                        // Pull old objects
                        var oldObjects = configurationWrapper.PreviousConfiguration.WrappedConfiguration.ObjectsMap.AllObjects;

                        // Pull new objects
                        var newObjects = configurationWrapper.LastAddedObjects;

                        // Call the analyzer
                        var theorems = _gradualAnalyzer.Analyze(oldObjects, newObjects);

                        // Unwrap configuration
                        var unwrappedConfiguration = configurationWrapper.WrappedConfiguration;

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