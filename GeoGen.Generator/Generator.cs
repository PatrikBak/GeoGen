using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Generator.ConfigurationHandling;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.Constructing;

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
        private readonly IConfigurationContainer _configurationContainer;

        /// <summary>
        /// The configuration handler.
        /// </summary>
        private readonly IConfigurationsHandler _configurationsHandler;

        /// <summary>
        /// The configuration constructer.
        /// </summary>
        private readonly IConfigurationConstructor _configurationConstructor;

        /// <summary>
        /// The maximal number of iterations that are supposed to be perfomed.
        /// </summary>
        private readonly int _maximalNumberOfIterations;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new generator with all it's needed dependencies and a given number of iterations.
        /// </summary>
        /// <param name="configurationContainer">The container</param>
        /// <param name="configurationConstructor">The configuration constructor</param>
        /// <param name="configurationsHandler">The configurations handler</param>
        /// <param name="maximalNumberOfIterations">The maximal number of iterations.</param>
        internal Generator(IConfigurationContainer configurationContainer, IConfigurationConstructor configurationConstructor,
            IConfigurationsHandler configurationsHandler, int maximalNumberOfIterations)
        {
            if (maximalNumberOfIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximalNumberOfIterations), "Number of iterations must be at least one");

            _maximalNumberOfIterations = maximalNumberOfIterations;
            _configurationContainer = configurationContainer ?? throw new ArgumentNullException(nameof(configurationConstructor));
            _configurationsHandler = configurationsHandler ?? throw new ArgumentNullException(nameof(configurationConstructor));
            _configurationConstructor = configurationConstructor ?? throw new ArgumentNullException(nameof(configurationConstructor));
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
            var newLayerConfigurations = _configurationContainer
                    // paralelize (TODO: Check real perfomance, thread safety)    
                    .AsParallel()
                    // create configurations and merge them
                    .SelectMany(c => _configurationConstructor.GenerateNewConfigurationObjects(c))
                    // convert to list
                    .ToList();

            // make container aware of the new layer
            _configurationContainer.AddLayer(newLayerConfigurations);

            // let the handler handle the container and lazily return the output
            foreach (var generatorOutput in _configurationsHandler.GenerateFinalOutput(_configurationContainer))
            {
                yield return generatorOutput;
            }
        }

        #endregion
    }
}