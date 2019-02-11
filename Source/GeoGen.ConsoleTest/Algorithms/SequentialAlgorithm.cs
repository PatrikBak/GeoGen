using GeoGen.Analyzer;
using GeoGen.Generator;
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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialAlgorithm"/> class.
        /// </summary>
        /// <param name="generator">The generator that generates configurations.</param>
        /// <param name="analyzer">The analyzer of relevant theorems in generated configurations.</param>
        public SequentialAlgorithm(IGenerator generator, IRelevantTheoremsAnalyzer analyzer)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
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
            return _generator.Generate(input).Select(output => new AlgorithmOutput
            {
                // Set the generator output
                GeneratorOutput = output,

                // Perform the theorem analysis
                AnalyzerOutput = _analyzer.Analyze(output.Configuration, output.Manager)
            });
        }

        #endregion
    }
}
