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
        /// The maximal number of iterations that are supposed to be performed.
        /// </summary>
        private readonly int _iterations;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="manager">The manager of generated configurations.</param>
        /// <param name="constructor">The constructor of new objects to configurations.</param>
        /// <param name="analyzer">The analyzer of generated configurations.</param>
        /// <param name="iterations">The number of iterations to be performed.</param>
        public Generator(IConfigurationsManager manager, IObjectsConstructor constructor, ITheoremsAnalyzer analyzer, int iterations)
        {
            _configurationsManager = manager ?? throw new ArgumentNullException(nameof(manager));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _objectsConstructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _iterations = iterations;
        }

        #endregion

        #region IGenerator methods

        /// <summary>
        /// Starts the generation process and lazily returns the output.
        /// </summary>
        /// <returns>The generator output enumerable.</returns>
        public IEnumerable<GeneratorOutput> Generate()
        {
            return Enumerable.Range(0, _iterations)
                    .SelectMany(iterationIndex =>
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