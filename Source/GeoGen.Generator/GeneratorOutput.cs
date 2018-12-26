using GeoGen.Core;
using System.Collections.Generic;

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
        /// Gets or sets the list of the theorems that holds true in the generated configuration.
        /// </summary>
        public List<Theorem> Theorems { get; set; }
    }
}