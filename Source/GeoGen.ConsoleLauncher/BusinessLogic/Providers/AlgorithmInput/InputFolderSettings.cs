using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="AlgorithmInputProvider"/> containing information
    /// about the folder structure for finding inputs and outputs.
    /// </summary>
    public class InputFolderSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the folder containing input files.
        /// </summary>
        public string InputFolderPath { get; }

        /// <summary>
        /// The prefix of inputs files.
        /// </summary>
        public string InputFilePrefix { get; }

        /// <summary>
        /// The file extension of input files.
        /// </summary>
        public string FileExtension { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InputFolderSettings"/> class.
        /// </summary>
        /// <param name="inputFolderPath">The path to the folder containing input files.</param>
        /// <param name="inputFilePrefix">The prefix of inputs files.</param>
        /// <param name="fileExtension">The file extension of input files.</param>
        public InputFolderSettings(string inputFolderPath, string inputFilePrefix, string fileExtension)
        {
            InputFolderPath = inputFolderPath ?? throw new ArgumentNullException(nameof(inputFolderPath));
            InputFilePrefix = inputFilePrefix ?? throw new ArgumentNullException(nameof(inputFilePrefix));
            FileExtension = fileExtension ?? throw new ArgumentNullException(nameof(fileExtension));
        }

        #endregion
    }
}
