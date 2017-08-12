using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IGenerator"/> service.
    /// </summary>
    internal class Generator : IGenerator
    {
        #region Private fields

        /// <summary>
        /// The context in which this generator lives.
        /// </summary>
        private readonly IGeneratorContext _generatorContext;

        /// <summary>
        /// The maximal number of iterations that are supposed to be perfomed.
        /// </summary>
        private readonly int _maximalNumberOfIterations;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new generator with a given context and a given number of iterations.
        /// </summary>
        /// <param name="generatorContext">The context.</param>
        /// <param name="maximalNumberOfIterations">The maximal number of iterations.</param>
        internal Generator(IGeneratorContext generatorContext, int maximalNumberOfIterations)
        {
            _generatorContext = generatorContext ?? throw new ArgumentNullException(nameof(generatorContext));

            if (maximalNumberOfIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximalNumberOfIterations), "Number of iterations must be at least one");

            _maximalNumberOfIterations = maximalNumberOfIterations;
        }

        #endregion

        #region Generation proccess

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
            var newLayerConfigurations = _generatorContext
                .ConfigurationContainer                        // pull configurations
                .Configurations                                // paralelize (TODO: Check real perfomance)    
                .AsParallel()                                  // create configurations and merge them
                .SelectMany(CreateConfigurationsOnNewLayer)    // convert to list
                .ToList();

            // make container aware of the new layer
            _generatorContext.ConfigurationContainer.AddLayer(newLayerConfigurations);

            // let the handler handle the container and lazily return the output
            foreach (var generatorOutput in _generatorContext.ConfigurationsHandler.GenerateFinalOutput())
            {
                yield return generatorOutput;
            }
        }

        /// <summary>
        /// Creates configurations on a new layer produces from a given configuration.
        /// </summary>
        /// <param name="configuration">The given configuration</param>
        /// <returns></returns>
        private IEnumerable<Configuration> CreateConfigurationsOnNewLayer(Configuration configuration)
        {
            return _generatorContext.ConfigurationConstructor.GenerateNewConfigurations(configuration);
        }

        #endregion
    }
}