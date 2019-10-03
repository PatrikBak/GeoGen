namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="AlgorithmRunner"/>
    /// </summary>
    public class AlgorithmRunnerSettings
    {
        /// <summary>
        /// Gets or sets the folder where the where the output of the algorithm should be written.
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// Gets or sets the extensions of files where the output of the algorithm should be written.
        /// </summary>
        public string OutputFileExtention { get; set; }

        /// <summary>
        /// Gets or sets the prefix of files where the output of the algorithm should be written.
        /// </summary>
        public object OutputFilePrefix { get; set; }

        /// <summary>
        /// Gets or sets how often we log the number of generated configurations.
        /// If this number is 'n', then after every n-th configuration there will be a message.
        /// </summary>
        public int GenerationProgresLoggingFrequency { get; set; }

        /// <summary>
        /// Gets or sets whether we should log the progress.
        /// </summary>
        public bool LogProgress { get; set; }

        /// <summary>
        /// Gets or sets whether we should generate a report containing even resolved theorems.
        /// </summary>
        public bool GenerateFullReport { get; set; }

        /// <summary>
        /// Gets or sets the suffix for the file containing the full report.
        /// </summary>
        public string FullReportSuffix { get; set; }

        /// <summary>
        /// Indicates if we should display attempts at proofs of theorems.
        /// </summary>
        public bool DisplayProofAttempts { get; set; }
    }
}
