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
        /// The folder where the where the human-readable output of the algorithm should be written.
        /// </summary>
        public string OutputFolder { get; }

        /// <summary>
        /// The folder where the where the human-readable output of the algorithm with theorem proofs should be written.
        /// </summary>
        public string OutputFolderWithProofs { get; }

        /// <summary>
        /// Indicates whether the output of the algorithm with proofs should be written.
        /// </summary>
        public bool WriteOutputWithProofs { get; }

        /// <summary>
        /// The folder where the output of the algorithm in JSON should be written. 
        /// </summary>
        public string OutputJsonFolder { get; }

        /// <summary>
        /// The prefix of all types of output files.
        /// </summary>
        public string OutputFilePrefix { get; }

        /// <summary>
        /// The extensions of all types of output files, except for JSON files, which has .json extension.
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// Indicates how often we log the number of generated configurations.
        /// If this number is 'n', then after every n-th configuration there will be a message.
        /// </summary>
        public int GenerationProgresLoggingFrequency { get; }

        /// <summary>
        /// Indicates whether we should log progress.
        /// </summary>
        public bool LogProgress { get; }

        /// <summary>
        /// The path to the human-readable file where the best theorems should be written to.
        /// </summary>
        public string BestTheoremsReadableFilePath { get; }

        /// <summary>
        /// The path to the JSON file where the best theorems should be written to.
        /// </summary>
        public string BestTheoremsJsonFilePath { get; }

        /// <summary>
        /// The file with the usage of the inference rules used for analyzing the prover.
        /// </summary>
        public string InferenceRuleUsageFile { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugAlgorithmRunnerSettings"/> class.
        /// </summary>
        /// <param name="outputFolder">The folder where the where the human-readable output of the algorithm should be written.</param>
        /// <param name="outputFolderWithProofs">The folder where the where the human-readable output of the algorithm with theorem proofs should be written.</param>
        /// <param name="writeOutputWithProofs">Indicates whether the output of the algorithm with proofs should be written.</param>
        /// <param name="outputJsonFolder">The folder where the output of the algorithm in JSON should be written.</param>
        /// <param name="outputFilePrefix">The prefix of all types of output files.</param>
        /// <param name="fileExtension">The extensions of all types of output files, except for JSON files, which has .json extension.</param>
        /// <param name="generationProgresLoggingFrequency">Indicates how often we log the number of generated configurations. If this number is 'n', then after every n-th configuration there will be a message.</param>
        /// <param name="logProgress">Indicates whether we should log progress.</param>
        /// <param name="bestTheoremsReadableFilePath">The path to the human-readable file where the best theorems should be written to.</param>
        /// <param name="bestTheoremsJsonFilePath">The path to the JSON file where the best theorems should be written to.</param>
        /// <param name="inferenceRuleUsageFile">The file with the usage of the inference rules used for analyzing the prover.</param>
        public DebugAlgorithmRunnerSettings(string outputFolder,
                                            string outputFolderWithProofs,
                                            bool writeOutputWithProofs,
                                            string outputJsonFolder,
                                            string outputFilePrefix,
                                            string fileExtension,
                                            int generationProgresLoggingFrequency,
                                            bool logProgress,
                                            string bestTheoremsReadableFilePath,
                                            string bestTheoremsJsonFilePath,
                                            string inferenceRuleUsageFile)
        {
            OutputFolder = outputFolder ?? throw new ArgumentNullException(nameof(outputFolder));
            OutputFolderWithProofs = outputFolderWithProofs ?? throw new ArgumentNullException(nameof(outputFolderWithProofs));
            WriteOutputWithProofs = writeOutputWithProofs;
            OutputJsonFolder = outputJsonFolder ?? throw new ArgumentNullException(nameof(outputJsonFolder));
            OutputFilePrefix = outputFilePrefix ?? throw new ArgumentNullException(nameof(outputFilePrefix));
            FileExtension = fileExtension ?? throw new ArgumentNullException(nameof(fileExtension));
            GenerationProgresLoggingFrequency = generationProgresLoggingFrequency;
            LogProgress = logProgress;
            BestTheoremsReadableFilePath = bestTheoremsReadableFilePath ?? throw new ArgumentNullException(nameof(bestTheoremsReadableFilePath));
            BestTheoremsJsonFilePath = bestTheoremsJsonFilePath ?? throw new ArgumentNullException(nameof(bestTheoremsJsonFilePath));
            InferenceRuleUsageFile = inferenceRuleUsageFile ?? throw new ArgumentNullException(nameof(inferenceRuleUsageFile));
        }

        #endregion
    }
}
