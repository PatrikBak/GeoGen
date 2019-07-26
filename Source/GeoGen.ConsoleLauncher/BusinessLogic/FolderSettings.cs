namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for the <see cref="FolderScanner"/> containing information
    /// about the folder structure for finding inputs, outputs and trivial theorems.
    /// </summary>
    public class FolderSettings
    {
        /// <summary>
        /// Gets or sets the path to the folder where all files we're working with are. 
        /// </summary>
        public string WorkingFolder { get; set; }

        /// <summary>
        /// Gets or sets the relative path to the folder where template theorems for sub-theorem algorithm are.
        /// This folder should be relative to the <see cref="WorkingFolder"/>.
        /// </summary>
        public string TemplateTheoremsFolder { get; set; }

        /// <summary>
        /// Gets or sets the prefix of files that should be considered inputs.
        /// </summary>
        public string InputFilePrefix { get; set; }

        /// <summary>
        /// GEts or sets the prefix of files where the output of the algorithm should go.
        /// </summary>
        public string OutputFilePrefix { get; set; }

        /// <summary>
        /// Gets the file extension of all involved files.
        /// </summary>
        public string FilesExtention { get; set; }
    }
}
