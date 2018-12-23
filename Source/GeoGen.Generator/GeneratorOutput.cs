using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents the output of a generator.
    /// </summary>
    public class GeneratorOutput
    {
        /// <summary>
        /// Gets or sets the configuration that has been generated.
        /// </summary>
        public GeneratedConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the list of the theorems that holds true in this configuration.
        /// </summary>
        public List<Theorem> Theorems { get; set; }
    }
}