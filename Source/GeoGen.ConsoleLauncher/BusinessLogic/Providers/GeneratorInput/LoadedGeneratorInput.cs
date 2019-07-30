using GeoGen.Generator;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a <see cref="GeneratorInput"/> loaded from a file.
    /// </summary>
    public class LoadedGeneratorInput : GeneratorInput
    {
        /// <summary>
        /// Gets or sets the path from which the input was loaded.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the id of the loaded input.
        /// </summary>
        public string Id { get; set; }
    }
}
