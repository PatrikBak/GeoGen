using GeoGen.TheoremsFinder;
using GeoGen.Generator;

namespace GeoGen.ConsoleTest
{
    /// <summary>
    /// Represents an output of the <see cref="IAlgorithm"/>.
    /// </summary>
    public class AlgorithmOutput
    {
        /// <summary>
        /// The output of the generator module.
        /// </summary>
        public GeneratorOutput GeneratorOutput { get; set; }

        /// <summary>
        /// The output of the analyzer module.
        /// </summary>
        public TheoremAnalysisOutput AnalyzerOutput { get; set; }
    }
}