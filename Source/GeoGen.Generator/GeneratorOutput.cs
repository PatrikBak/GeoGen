using GeoGen.Analyzer;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an output of the <see cref="Generator"/>.
    /// </summary>
    public class GeneratorOutput
    {
        /// <summary>
        /// Gets or sets the configuration that was generated.
        /// </summary>
        public GeneratedConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the output of the theorem analysis module.
        /// </summary>
        public TheoremsAnalyzerOutput AnalyzerOutput { get; set; }
    }
}