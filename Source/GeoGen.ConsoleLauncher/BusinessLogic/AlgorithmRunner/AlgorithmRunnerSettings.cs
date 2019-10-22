using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="AlgorithmRunner"/>
    /// </summary>
    public class AlgorithmRunnerSettings
    {
        #region Public properties

        /// <summary>
        /// The folder where the where the output of the algorithm should be written.
        /// </summary>
        public string OutputFolder { get; }

        /// <summary>
        /// The extensions of files where the output of the algorithm should be written.
        /// </summary>
        public string OutputFileExtention { get; }

        /// <summary>
        /// The prefix of files where the output of the algorithm should be written.
        /// </summary>
        public string OutputFilePrefix { get; }

        /// <summary>
        /// Indicates how often we log the number of generated configurations.
        /// If this number is 'n', then after every n-th configuration there will be a message.
        /// </summary>
        public int GenerationProgresLoggingFrequency { get; }

        /// <summary>
        /// Indicates whether we should log the progress.
        /// </summary>
        public bool LogProgress { get; }

        /// <summary>
        /// Indicates whether we should generate a report containing even resolved theorems.
        /// </summary>
        public bool GenerateFullReport { get; }

        /// <summary>
        /// The suffix for the file containing the full report.
        /// </summary>
        public string FullReportSuffix { get; }

        /// <summary>
        /// Indicates if we should display attempts at proofs of theorems.
        /// </summary>
        public bool DisplayProofAttempts { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmRunnerSettings"/> class.
        /// </summary>
        /// <param name="outputFolder">The folder where the where the output of the algorithm should be written.</param>
        /// <param name="outputFileExtention">The extensions of files where the output of the algorithm should be written.</param>
        /// <param name="outputFilePrefix">The prefix of files where the output of the algorithm should be written.</param>
        /// <param name="generationProgresLoggingFrequency">Indicates how often we log the number of generated configurations.</param>
        /// <param name="logProgress">Indicates whether we should log the progress.</param>
        /// <param name="generateFullReport">Indicates whether we should generate a report containing even resolved theorems.</param>
        /// <param name="fullReportSuffix">The suffix for the file containing the full report.</param>
        /// <param name="displayProofAttempts">Indicates if we should display attempts at proofs of theorems.</param>
        public AlgorithmRunnerSettings(string outputFolder,
                                       string outputFileExtention,
                                       string outputFilePrefix,
                                       int generationProgresLoggingFrequency,
                                       bool logProgress,
                                       bool generateFullReport,
                                       string fullReportSuffix,
                                       bool displayProofAttempts)
        {
            OutputFolder = outputFolder ?? throw new ArgumentNullException(nameof(outputFolder));
            OutputFileExtention = outputFileExtention ?? throw new ArgumentNullException(nameof(outputFileExtention));
            OutputFilePrefix = outputFilePrefix ?? throw new ArgumentNullException(nameof(outputFilePrefix));
            GenerationProgresLoggingFrequency = generationProgresLoggingFrequency;
            LogProgress = logProgress;
            GenerateFullReport = generateFullReport;
            FullReportSuffix = fullReportSuffix ?? throw new ArgumentNullException(nameof(fullReportSuffix));
            DisplayProofAttempts = displayProofAttempts;
        }

        #endregion
    }
}
