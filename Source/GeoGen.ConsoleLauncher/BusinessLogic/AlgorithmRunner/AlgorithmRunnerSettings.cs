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
        /// The folder where the where the output of the algorithm with attempts at unproven 
        /// theorems should be written.
        /// </summary>
        public string OutputWithAttemptsFolder { get; }

        /// <summary>
        /// Indicates whether the output of the algorithm with attempts at unproven theorems 
        /// should be written.
        /// </summary>
        public bool WriteOutputWithAttempts { get; }

        /// <summary>
        /// The folder where the where the output of the algorithm with attempts at unproven 
        /// theorems and proofs of proved ones should be written.
        /// </summary>
        public string OutputWithAttemptsAndProofsFolder { get; }

        /// <summary>
        /// Indicates whether the output of the algorithm with attempts at unproven theorems 
        /// and proofs of proved ones should be written.
        /// </summary>
        public bool WriteOutputWithAttemptsAndProofs { get; }

        /// <summary>
        /// The prefix for all types of output files.
        /// </summary>
        public string OutputFilePrefix { get; }

        /// <summary>
        /// The extensions of all types of output files.
        /// </summary>
        public string FilesExtention { get; }

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
        /// Indicates whether we should include discovered unproven theorems that were part of 
        /// proof attempts at the main (unproven) theorems.
        /// </summary>
        public bool IncludeUnprovenDiscoveredTheorems { get; }

        /// <summary>
        /// The path to the file where the best theorems should be written to.
        /// </summary>
        public string BestTheoremsFilePath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmRunnerSettings"/> class.
        /// </summary>
        /// <param name="outputFolder">The folder where the where the output of the algorithm should be written.</param>
        /// <param name="outputWithAttemptsFolder">The folder where the where the output of the algorithm with attempts at unproven theorems should be written.</param>
        /// <param name="writeOutputWithAttempts">Indicates whether the output of the algorithm with attempts at unproven theorems should be written.</param>
        /// <param name="outputWithAttemptsAndProofsFolder">The folder where the where the output of the algorithm with attempts at unproven theorems and proofs of proved ones should be written.</param>
        /// <param name="writeOutputWithAttemptsAndProofs">Indicates whether the output of the algorithm with attempts at unproven theorems and proofs of proved ones should be written.</param>
        /// <param name="outputFilePrefix">The prefix for all types of output files.</param>
        /// <param name="filesExtention">The extensions of all types of output files.</param>
        /// <param name="generationProgresLoggingFrequency">Indicates how often we log the number of generated configurations. If this number is 'n', then after every n-th configuration there will be a message.</param>
        /// <param name="logProgress">Indicates whether we should log the progress.</param>
        /// <param name="includeUnprovenDiscoveredTheorems">Indicates whether we should include discovered unproven theorems that were part of proof attempts at the main (unproven) theorems.</param>
        /// <param name="bestTheoremsFilePath">The path to the file where the best theorems should be written to.</param>
        public AlgorithmRunnerSettings(string outputFolder,
                                       string outputWithAttemptsFolder,
                                       bool writeOutputWithAttempts,
                                       string outputWithAttemptsAndProofsFolder,
                                       bool writeOutputWithAttemptsAndProofs,
                                       string outputFilePrefix,
                                       string filesExtention,
                                       int generationProgresLoggingFrequency,
                                       bool logProgress,
                                       bool includeUnprovenDiscoveredTheorems,
                                       string bestTheoremsFilePath)
        {
            OutputFolder = outputFolder ?? throw new ArgumentNullException(nameof(outputFolder));
            OutputWithAttemptsFolder = outputWithAttemptsFolder ?? throw new ArgumentNullException(nameof(outputWithAttemptsFolder));
            WriteOutputWithAttempts = writeOutputWithAttempts;
            OutputWithAttemptsAndProofsFolder = outputWithAttemptsAndProofsFolder ?? throw new ArgumentNullException(nameof(outputWithAttemptsAndProofsFolder));
            WriteOutputWithAttemptsAndProofs = writeOutputWithAttemptsAndProofs;
            OutputFilePrefix = outputFilePrefix ?? throw new ArgumentNullException(nameof(outputFilePrefix));
            FilesExtention = filesExtention ?? throw new ArgumentNullException(nameof(filesExtention));
            GenerationProgresLoggingFrequency = generationProgresLoggingFrequency;
            LogProgress = logProgress;
            IncludeUnprovenDiscoveredTheorems = includeUnprovenDiscoveredTheorems;
            BestTheoremsFilePath = bestTheoremsFilePath ?? throw new ArgumentNullException(nameof(bestTheoremsFilePath));
        }

        #endregion
    }
}
