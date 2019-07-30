namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="GeneratorInputsProvider"/> containing information
    /// about the folder structure for finding inputs and outputs.
    /// </summary>
    public class InputFolderSettings
    {
        /// <summary>
        /// Gets or sets the path to the folder containing input files.
        /// </summary>
        public string InputFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the prefix of inputs files.
        /// </summary>
        public string InputFilePrefix { get; set; }

        /// <summary>
        /// Gets the file extension of input files.
        /// </summary>
        public string FilesExtention { get; set; }
    }
}
