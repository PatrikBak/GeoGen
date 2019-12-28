using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="DebugAlgorithmRunner"/>
    /// </summary>
    public class DebugAlgorithmRunnerSettings
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
        /// The folder where the output of the algorithm in JSON should be written. This output
        /// is easily readable backwards by a program.
        /// </summary>
        public string OutputJsonFolder { get; }

        /// <summary>
        /// The prefix for all types of output files.
        /// </summary>
        public string OutputFilePrefix { get; }

        /// <summary>
        /// The extensions of all types of output files, except for JSON files, which has .json extension.
        /// </summary>
        public string FilesExtension { get; }

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
        /// The path to the human-readable file where the best theorems should be written to.
        /// </summary>
        public string BestTheoremsReadableFilePath { get; }

        /// <summary>
        /// The path to the JSON file where the best theorems should be written to.
        /// </summary>
        public string BestTheoremsJsonFilePath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugAlgorithmRunnerSettings"/> class.
        /// </summary>
        /// <param name="outputFolder">The folder where the where the output of the algorithm should be written.</param>
        /// <param name="outputWithAttemptsFolder">The folder where the where the output of the algorithm with attempts at unproven theorems should be written.</param>
        /// <param name="writeOutputWithAttempts">Indicates whether the output of the algorithm with attempts at unproven theorems should be written.</param>
        /// <param name="outputWithAttemptsAndProofsFolder">The folder where the where the output of the algorithm with attempts at unproven theorems and proofs of proved ones should be written.</param>
        /// <param name="writeOutputWithAttemptsAndProofs">Indicates whether the output of the algorithm with attempts at unproven theorems and proofs of proved ones should be written.</param>
        /// <param name="outputJsonFolder">he folder where the output of the algorithm in JSON should be written. This output is easily readable backwards by a program.</param>
        /// <param name="outputFilePrefix">The prefix for all types of output files.</param>
        /// <param name="filesExtension">The extensions of all types of output files, except for JSON files, which has .json extension.</param>
        /// <param name="generationProgresLoggingFrequency">Indicates how often we log the number of generated configurations. If this number is 'n', then after every n-th configuration there will be a message.</param>
        /// <param name="logProgress">Indicates whether we should log the progress.</param>
        /// <param name="includeUnprovenDiscoveredTheorems">Indicates whether we should include discovered unproven theorems that were part of proof attempts at the main (unproven) theorems.</param>
        /// <param name="bestTheoremsReadableFilePath">The path to the human-readable file where the best theorems should be written to.</param>
        /// <param name="bestTheoremsJsonFilePath">The path to the JSON file where the best theorems should be written to.</param>
        public DebugAlgorithmRunnerSettings(string outputFolder,
                                            string outputWithAttemptsFolder,
                                            bool writeOutputWithAttempts,
                                            string outputWithAttemptsAndProofsFolder,
                                            bool writeOutputWithAttemptsAndProofs,
                                            string outputJsonFolder,
                                            string outputFilePrefix,
                                            string filesExtension,
                                            int generationProgresLoggingFrequency,
                                            bool logProgress,
                                            bool includeUnprovenDiscoveredTheorems,
                                            string bestTheoremsReadableFilePath,
                                            string bestTheoremsJsonFilePath)
        {
            OutputFolder = outputFolder ?? throw new ArgumentNullException(nameof(outputFolder));
            OutputWithAttemptsFolder = outputWithAttemptsFolder;
            WriteOutputWithAttempts = writeOutputWithAttempts;
            OutputWithAttemptsAndProofsFolder = outputWithAttemptsAndProofsFolder;
            WriteOutputWithAttemptsAndProofs = writeOutputWithAttemptsAndProofs;
            OutputJsonFolder = outputJsonFolder ?? throw new ArgumentNullException(nameof(outputJsonFolder));
            OutputFilePrefix = outputFilePrefix ?? throw new ArgumentNullException(nameof(outputFilePrefix));
            FilesExtension = filesExtension ?? throw new ArgumentNullException(nameof(filesExtension));
            GenerationProgresLoggingFrequency = generationProgresLoggingFrequency;
            LogProgress = logProgress;
            IncludeUnprovenDiscoveredTheorems = includeUnprovenDiscoveredTheorems;
            BestTheoremsReadableFilePath = bestTheoremsReadableFilePath ?? throw new ArgumentNullException(nameof(bestTheoremsReadableFilePath));
            BestTheoremsJsonFilePath = bestTheoremsJsonFilePath ?? throw new ArgumentNullException(nameof(bestTheoremsJsonFilePath));
        }

        #endregion
    }
}
