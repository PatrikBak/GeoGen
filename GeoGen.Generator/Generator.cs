using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;
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
        /// <param name="configurationContainer">The container</param>
        /// <param name="objectsConstructor">The configuration constructor</param>
        /// <param name="configurationsHandler">The configurations handler</param>
        /// <param name="maximalNumberOfIterations">The maximal number of iterations.</param>
        internal Generator(IConfigurationContainer configurationContainer, IObjectsConstructor objectsConstructor,
                           IConfigurationsHandler configurationsHandler, int maximalNumberOfIterations)
        {
            if (maximalNumberOfIterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximalNumberOfIterations), "Number of iterations must be at least one");

            _maximalNumberOfIterations = maximalNumberOfIterations;
            _configurationContainer = configurationContainer ?? throw new ArgumentNullException(nameof(objectsConstructor));
            _configurationsHandler = configurationsHandler ?? throw new ArgumentNullException(nameof(objectsConstructor));
            _objectsConstructor = objectsConstructor ?? throw new ArgumentNullException(nameof(objectsConstructor));
        }

        private static readonly Object obj = new Object();

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
            //var newLayerConfigurations = _configurationContainer
            //        // get the current layer
            //        .CurrentLayer
            //        // create configurations and merge them
            //        .Select(c => _objectsConstructor.GenerateNewConfigurationObjects(c))
            //        .ToList();
            // convert to list

            var bag = new ConcurrentBag<IEnumerable<ConstructorOutput>>();

            //Console.WriteLine(newLayerConfigurations.Count());

            //foreach (var output in newLayerConfigurations)
            {
                //bag.Add(output);
            }

            Console.WriteLine(bag.Count);
                    

            
            //Foreach
            //(
            //    _configurationContainer.CurrentLayer, arg =>
            //    {
            //        lock (obj)
            //        {
            //            bag.Add(_objectsConstructor.GenerateNewConfigurationObjects((ConfigurationWrapper) arg));
            //        }
            //    }
            //);

            //var a = bag.SelectMany(s => s).ToList();
            var p = 0;
            var i = new List<ConstructorOutput>();

            Parallel.ForEach(
                _configurationContainer.CurrentLayer, j =>
                {
                    lock (obj)
                    {
                        p++;
                        foreach (var constructorOutput in _objectsConstructor.GenerateNewConfigurationObjects((ConfigurationWrapper) j))
                        {
                            i.Add(constructorOutput);
                        }
                    }
                });
            
            Console.WriteLine(p);

            // make container aware of the new layer
            _configurationContainer.AddLayer(i);

            // get the current (new) layer
            var currentLayer = _configurationContainer.CurrentLayer;

            // let the handler handle the container and lazily return the output
            foreach (var generatorOutput in _configurationsHandler.GenerateFinalOutput(currentLayer))
            {
                yield return generatorOutput;
            }
        }

        private void ForEach(List<ConfigurationWrapper> configurationContainerCurrentLayer, Action<object> action)
        {
            foreach (var configurationWrapper in configurationContainerCurrentLayer)
            {
                action(configurationWrapper);
            }
        }

        private void Foreach(List<ConfigurationWrapper> configurationContainerCurrentLayer, Action<object> action)
        {
            foreach (var configurationWrapper in configurationContainerCurrentLayer)
            {
                action(configurationWrapper);
            }
        }

        #endregion
    }
}