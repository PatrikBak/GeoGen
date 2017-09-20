using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IGenerator"/> interface.
    /// </summary>
    internal class Generator : IGenerator
    {
        #region Private fields

        /// <summary>
        /// The configuration container.
        /// </summary>
        private readonly IConfigurationsContainer _configurationsContainer;

        /// <summary>
        /// The configuration handler.
        /// </summary>
        private readonly IConfigurationsHandler _configurationsHandler;

        /// <summary>
        /// The configuration constructer.
        /// </summary>
        private readonly IObjectsConstructor _objectsConstructor;

        /// <summary>
        /// The maximal number of iterations that are supposed to be perfomed.
        /// </summary>
        private readonly int _maximalNumberOfIterations;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new generator with all it's needed dependencies and a given number of iterations.
        /// </summary>
        /// <param name="configurationsContainer">The container</param>
        /// <param name="objectsConstructor">The configuration constructor</param>
        /// <param name="configurationsHandler">The configurations handler</param>
        /// <param name="maximalNumberOfIterations">The maximal number of iterations.</param>
        internal Generator(IConfigurationsContainer configurationsContainer, IObjectsConstructor objectsConstructor,
                           IConfigurationsHandler configurationsHandler, int maximalNumberOfIterations)
        {
            if (maximalNumberOfIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximalNumberOfIterations), "Number of iterations must be at least one");

            _maximalNumberOfIterations = maximalNumberOfIterations;
            _configurationsContainer = configurationsContainer ?? throw new ArgumentNullException(nameof(objectsConstructor));
            _configurationsHandler = configurationsHandler ?? throw new ArgumentNullException(nameof(objectsConstructor));
            _objectsConstructor = objectsConstructor ?? throw new ArgumentNullException(nameof(objectsConstructor));
        }

        #endregion

        #region IGenerator implementation

        /// <summary>
        /// Starts the generation proccess and lazily return the output.
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

        /// <summary>
        /// Generates the output for the current iteration.
        /// </summary>
        /// <returns>The output.</returns>
        private IEnumerable<GeneratorOutput> GenerateOutputInCurrentIteration()
        {
            // TODO: Parallel ForEach with ConcurrentBag seems faster
            var newLayerConfigurations = _configurationsContainer
                    // get the current layer
                    .CurrentLayer
                    //.AsParallel() // 37 seconds 
                    // create configurations and merge them
                    .SelectMany(d => _objectsConstructor.GenerateOutput(d))
                    // convert to list
                    .ToList();
            
            // make container aware of the new layer
            _configurationsContainer.AddLayer(newLayerConfigurations);

            // get the current (new) layer
            var currentLayer = _configurationsContainer.CurrentLayer;

            // let the handler handle the container and lazily return the output
            foreach (var generatorOutput in _configurationsHandler.GenerateFinalOutput(currentLayer))
            {
                yield return generatorOutput;
            }
        }

        #endregion
    }
}