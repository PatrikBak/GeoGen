using GeoGen.Constructor;

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
        /// Gets or sets the manager holding the geometric representation of the configuration.
        /// </summary>
        public IPicturesManager Manager { get; set; }

        /// <summary>
        /// Gets the index of the iteration on which the output was produces, starting with 0.
        /// </summary>
        public int IterationIndex { get; set; }
    }
}