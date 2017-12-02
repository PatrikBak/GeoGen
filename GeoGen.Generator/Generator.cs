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
            return Enumerable.Range(0, _maximalNumberOfIterations).SelectMany
            (
                iterationIndex =>
                {
                    // Create the output for new layer
                    var outputForNewLayer = _configurationsContainer
                            // Get the current layer
                            .CurrentLayer
                            // Take only not excluded configurations
                            .Where(configuration => !configuration.Excluded)
                            // Construct configurations and merge them
                            .SelectMany(configuration => _objectsConstructor.GenerateOutput(configuration));

                    // Create the configurations on new layer
                    var newLayersConfigurations = _configurationsContainer.AddLayer(outputForNewLayer);

                    // Let the handle process the configurations to obtain the output
                    return _configurationsHandler.GenerateFinalOutput(newLayersConfigurations);
                }
            );
        }

        #endregion
    }
}