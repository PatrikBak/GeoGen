using GeoGen.Constructor;
using GeoGen.Generator;
using GeoGen.TheoremsFinder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.ConsoleTest
{
    /// <summary>
    /// Represents a simple version of the algorithm where each configuration is tested for theorems immediately
    /// after its generated
    /// </summary>
    public class SequentialAlgorithm : IAlgorithm
    {
        #region Dependencies

        /// <summary>
        /// The generator that generates configurations.
        /// </summary>
        private readonly IGenerator _generator;

        /// <summary>
        /// The analyzer of relevant theorems in generated configurations.
        /// </summary>
        private readonly IRelevantTheoremsAnalyzer _analyzer;

        /// <summary>
        /// The factory for creating contextual pictures that are required by the relevant theorem analyzer.
        /// </summary>
        private readonly IContextualPictureFactory _factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialAlgorithm"/> class.
        /// </summary>
        /// <param name="generator">The generator that generates configurations.</param>
        /// <param name="analyzer">The analyzer of relevant theorems in generated configurations.</param>
        /// <param name="factory">The factory for creating contextual pictures that are required by the relevant theorem analyzer.</param>
        public SequentialAlgorithm(IGenerator generator, IRelevantTheoremsAnalyzer analyzer, IContextualPictureFactory factory)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region IAlgorithm implementation

        /// <summary>
        /// Executes the algorithm for a given generator input.
        /// </summary>
        /// <param name="input">The input for the generator module of the algorithm.</param>
        /// <returns>A lazy enumerable of all the generated output.</returns>
        public IEnumerable<AlgorithmOutput> Execute(GeneratorInput input)
        {
            // Perform the generation
            return _generator.Generate(input).Select(output =>
            {
                // Prepare a variable holding the contextual picture to be used by the analyzers
                IContextualPicture picture = null;

                try
                {
                    // Let the manager safely create an instance of the contextual picture
                    picture = _factory.Create(output.Configuration, output.Manager);
                }
                catch (InconstructibleContextualPicture)
                {
                    // If we cannot create a contextual picture, we cannot do much
                }

                // Return the output together with the constructed picture
                return (output, picture);
            })
            // Take only such pairs where the picture was successfully created
            .Where(pair => pair.picture != null)
            // For each such pair perform the theorem analysis
            .Select(pair => new AlgorithmOutput
            {
                // Set the generator output
                GeneratorOutput = pair.output,

                // Perform the theorem analysis
                AnalyzerOutput = _analyzer.Analyze(pair.output.Configuration, pair.output.Manager, pair.picture)
            });
        }

        #endregion
    }
}
