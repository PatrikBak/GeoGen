using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Generator.ConfigurationsHandling;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IGenerator"/>.
    /// </summary>
    internal sealed class Generator : IGenerator
    {
        #region Private fields

        /// <summary>
        /// The configurations container.
        /// </summary>
        private readonly IConfigurationsContainer _configurationsContainer;

        /// <summary>
        /// The configurations handler.
        /// </summary>
        private readonly IConfigurationsHandler _configurationsHandler;

        /// <summary>
        /// The configuration constructor.
        /// </summary>
        private readonly IObjectsConstructor _objectsConstructor;

        /// <summary>
        /// The maximal number of iterations that are supposed to be performed.
        /// </summary>
        private readonly int _maximalNumberOfIterations;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new generator. The generator uses the objects constructor for 
        /// creating new configuration objects, the configurations container for
        /// registering them (it's supposed be already initialized) and the configurations
        /// handler for handling constructed configurations (passing them to the analyzer and
        /// processing its output). Also a number of iterations that are to be performed is specified.
        /// </summary>
        /// <param name="configurationsContainer">The container</param>
        /// <param name="objectsConstructor">The configuration constructor</param>
        /// <param name="configurationsHandler">The configurations handler</param>
        /// <param name="maximalNumberOfIterations">The maximal number of iterations.</param>
        public Generator
        (
            IConfigurationsContainer configurationsContainer,
            IObjectsConstructor objectsConstructor,
            IConfigurationsHandler configurationsHandler,
            int maximalNumberOfIterations
        )
        {
            if (maximalNumberOfIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximalNumberOfIterations), "Number of iterations must be at least one");

            _maximalNumberOfIterations = maximalNumberOfIterations;
            _configurationsContainer = configurationsContainer ?? throw new ArgumentNullException(nameof(configurationsContainer));
            _configurationsHandler = configurationsHandler ?? throw new ArgumentNullException(nameof(configurationsHandler));
            _objectsConstructor = objectsConstructor ?? throw new ArgumentNullException(nameof(objectsConstructor));
        }

        #endregion

        #region IGenerator methods

        /// <summary>
        /// Starts the generation process and lazily returns the output.
        /// </summary>
        /// <returns>The generator output enumerable.</returns>
        public IEnumerable<GeneratorOutput> Generate()
        {
            // Iterate for the given number of times
            for (var index = 0; index < _maximalNumberOfIterations; index++)
            {
                // Iterate through the output of the current iteration
                foreach (var generatorOutput in GenerateOutputInCurrentIteration())
                {
                    yield return generatorOutput;
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Generates the output for the current iteration.
        /// </summary>
        /// <returns>The output.</returns>
        private IEnumerable<GeneratorOutput> GenerateOutputInCurrentIteration()
        {
            var newLayerConfigurations = _configurationsContainer
                    // Get the current layer
                    .CurrentLayer
                    // Create configurations and merge them
                    .SelectMany(d => _objectsConstructor.GenerateOutput(d))
                    // Enumerate them to list
                    .ToList();

            // Make the container aware of the new layer
            _configurationsContainer.AddLayer(newLayerConfigurations);

            // Pull the current (new) layer
            var currentLayer = _configurationsContainer.CurrentLayer;

            // Let the handler handle the container and lazily return the output
            foreach (var generatorOutput in _configurationsHandler.GenerateFinalOutput(currentLayer))
            {
                yield return generatorOutput;
            }
        }

        #endregion
    }
}