using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer;
using GeoGen.Core.Generator;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IGenerator"/>.
    /// </summary>
    internal sealed class Generator : IGenerator
    {
        #region Private fields

        /// <summary>
        /// The configurations manager.
        /// </summary>
        private readonly IConfigurationsManager _configurationsManager;

        /// <summary>
        /// The configuration constructor.
        /// </summary>
        private readonly IObjectsConstructor _objectsConstructor;

        private readonly IGradualAnalyzer _gradualAnalyzer;

        /// <summary>
        /// The maximal number of iterations that are supposed to be performed.
        /// </summary>
        private readonly int _maximalNumberOfIterations;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new generator. The generator uses the objects constructor for 
        /// creating new configuration objects, the configurations container that is able 
        /// to create new configurations from those objects, and the configurations handler 
        /// that passes created configurations to the analyzer and processes its output. 
        /// </summary>
        /// <param name="configurationsManager">The configurations manager.</param>
        /// <param name="objectsConstructor">The objects constructor.</param>
        /// <param name="configurationsHandler">The configurations handler.</param>
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

            if (maximalNumberOfIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximalNumberOfIterations), "Number of iterations must be at least one");

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
            return Enumerable.Range(0, _maximalNumberOfIterations)
                    .SelectMany
                    (
                            iterationIndex =>
                            {
                                // From the container
                                var outputForNewLayer = _configurationsManager
                                        // Take the current layer
                                        .CurrentLayer
                                        // Take only not excluded configurations
                                        .Where(configuration => !configuration.Excluded)
                                        // Construct outputs from each of them
                                        .SelectMany(configuration => _objectsConstructor.GenerateOutput(configuration));

                                // Take the enumerable for creating the new layer
                                return _configurationsManager.AddLayer(outputForNewLayer);
                            }
                    )
                    .Select
                    (
                            configurationWrapper =>
                            {
                                // Pull old objects
                                var oldObjects = configurationWrapper.OriginalObjects;

                                // Pull new objects
                                var newObjects = configurationWrapper.LastAddedObjects;

                                // Call the analyzer
                                var theorems = _gradualAnalyzer.Analyze(oldObjects, newObjects);

                                // Unwrap configuration
                                var unwrappedConfiguration = configurationWrapper.Configuration;

                                // Return the output
                                return new GeneratorOutput
                                {
                                        Configuration = unwrappedConfiguration,
                                        Theorems = theorems
                                };
                            }
                    )
                    .Where(output => !output.Theorems.Empty());
        }

        #endregion
    }
}