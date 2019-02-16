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
        /// Gets or sets the manager holding the geometric representations. of the configurations.
        /// </summary>
        public IObjectsContainersManager Manager { get; set; }
    }
}