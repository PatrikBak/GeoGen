namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The settings for <see cref="InvalidInferenceTracer"/>.
    /// </summary>
    public class InvalidInferenceTracerSettings
    {
        #region Public properties

        /// <summary>
        /// The folder where inferences using particular rules will be written to.
        /// </summary>
        public string InvalidInferenceFolder { get; }

        /// <summary>
        /// The file extensions of the created files containing invalid inferences.
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// The number of invalid inferences that can be written to a single file. Others will be ignored.
        /// </summary>
        public int MaximalNumberOfInvalidInferencesPerFile { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInferenceTracerSettings"/> class.
        /// </summary>
        /// <param name="invalidInferenceFolder">The folder where inferences using particular rules will be written to.</param>
        /// <param name="fileExtension">The file extensions of the created files containing invalid inferences.</param>
        /// <param name="maximalNumberOfInvalidInferencesPerFile">The number of invalid inferences that can be written to a single file. Others will be ignored.</param>
        public InvalidInferenceTracerSettings(string invalidInferenceFolder, string fileExtension, int maximalNumberOfInvalidInferencesPerFile)
        {
            InvalidInferenceFolder = invalidInferenceFolder ?? throw new ArgumentNullException(nameof(invalidInferenceFolder));
            FileExtension = fileExtension ?? throw new ArgumentNullException(nameof(fileExtension));
            MaximalNumberOfInvalidInferencesPerFile = maximalNumberOfInvalidInferencesPerFile;

            // Ensure the maximal number of invalid inferences per file is positive
            if (MaximalNumberOfInvalidInferencesPerFile < 0)
                throw new ArgumentOutOfRangeException(nameof(maximalNumberOfInvalidInferencesPerFile), "The number of maximal invalid inferences per file must be at least 1.");
        }

        #endregion
    }
}
