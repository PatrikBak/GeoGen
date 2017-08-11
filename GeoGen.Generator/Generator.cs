using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;

namespace GeoGen.Generator
{
    internal class Generator : IGenerator
    {
        #region Private fields

        private readonly IGeneratorContext _generatorContext;

        private readonly int _maximalNumberOfIterations;

        #endregion

        #region Constructor

        internal Generator(IGeneratorContext generatorContext, int maximalNumberOfIterations)
        {
            _generatorContext = generatorContext ?? throw new ArgumentNullException(nameof(generatorContext));

            if (maximalNumberOfIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximalNumberOfIterations), "Number of iterations must be at least one");

            _maximalNumberOfIterations = maximalNumberOfIterations;
        }

        #endregion

        #region Generation proccess

        public IEnumerable<GeneratorOutput> Generate()
        {
            for (var index = 0; index < _maximalNumberOfIterations; index++)
            {
                foreach (var generatorOutput in GenerateOutputInCurrentIteration())
                {
                    yield return generatorOutput;
                }
            }
        }

        private IEnumerable<GeneratorOutput> GenerateOutputInCurrentIteration()
        {
            var newLayerConfigurations = _generatorContext
                .ConfigurationContainer
                .Configurations
                .AsParallel()
                .SelectMany(CreateConfigurationsOnNewLayer)
                .ToList();

            _generatorContext.ConfigurationContainer.AddNewLayer(newLayerConfigurations);

            foreach (var generatorOutput in _generatorContext.ConfigurationsHandler.GenerateFinalOutput())
            {
                yield return generatorOutput;
            }
        }

        private IEnumerable<Configuration> CreateConfigurationsOnNewLayer(Configuration configuration)
        {
            return _generatorContext.ConfigurationConstructer.GenerateNewConfigurations(configuration);
        }

        #endregion
    }
}