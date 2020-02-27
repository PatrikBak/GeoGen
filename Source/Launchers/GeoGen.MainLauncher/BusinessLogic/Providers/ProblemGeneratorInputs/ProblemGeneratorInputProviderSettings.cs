using System;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The settings for <see cref="ProblemGeneratorInputProvider"/> containing information about the folder with inputs.
    /// </summary>
    public class ProblemGeneratorInputProviderSettings
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
        /// Initializes a new instance of the <see cref="ProblemGeneratorInputProviderSettings"/> class.
        /// </summary>
        /// <param name="inputFolderPath">The path to the folder containing input files.</param>
        /// <param name="inputFilePrefix">The prefix of inputs files.</param>
        /// <param name="fileExtension">The file extension of input files.</param>
        public ProblemGeneratorInputProviderSettings(string inputFolderPath, string inputFilePrefix, string fileExtension)
        {
            InputFolderPath = inputFolderPath ?? throw new ArgumentNullException(nameof(inputFolderPath));
            InputFilePrefix = inputFilePrefix ?? throw new ArgumentNullException(nameof(inputFilePrefix));
            FileExtension = fileExtension ?? throw new ArgumentNullException(nameof(fileExtension));
        }

        #endregion
    }
}
