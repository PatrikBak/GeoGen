using GeoGen.ProblemAnalyzer;
using System;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The settings for <see cref="ProblemGenerationRunner"/>
    /// </summary>
    public class ProblemGenerationRunnerSettings
    {
        #region Public properties

        /// <summary>
        /// The folder where human-readable output without proofs should be written. The output files will have the
        /// prefix <see cref="OutputFilePrefix"/> followed by the corresponding input file name and then the extension
        /// <see cref="FileExtension"/>.
        /// </summary>
        public string ReadableOutputWithoutProofsFolder { get; }

        /// <summary>
        /// Indicates whether we should write human readable output without proofs.
        /// </summary>
        public bool WriteReadableOutputWithoutProofs { get; }

        /// <summary>
        /// The folder where human-readable output with proofs should be written. The output files will have the 
        /// prefix <see cref="OutputFilePrefix"/> followed by the corresponding input file name and then the extension
        /// <see cref="FileExtension"/>.
        /// </summary>
        public string ReadableOutputWithProofsFolder { get; }

        /// <summary>
        /// Indicates whether we should write human-readable output with proofs.
        /// </summary>
        public bool WriteReadableOutputWithProofs { get; }

        /// <summary>
        /// The folder where output in JSON should be written. The output files will have the prefix <see cref="OutputFilePrefix"/>
        /// followed by the corresponding input file name and then the extension 'json'.
        /// </summary>
        public string JsonOutputFolder { get; }

        /// <summary>
        /// Indicates whether we should write output in JSON. 
        /// </summary>
        public bool WriteJsonOutput { get; }

        /// <summary>
        /// The prefix of all types of output files described <see cref="ReadableOutputWithoutProofsFolder"/>, 
        /// <see cref="ReadableOutputWithProofsFolder"/> and <see cref="JsonOutputFolder"/>.
        /// </summary>
        public string OutputFilePrefix { get; }

        /// <summary>
        /// The extensions of all types of output files except for JSON files, i.e. files described in
        /// <see cref="ReadableOutputWithoutProofsFolder"/> and <see cref="ReadableOutputWithProofsFolder"/>.
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// The path to the human-readable file where the best theorems should be written to.
        /// </summary>
        public string ReadableBestTheoremFilePath { get; }

        /// <summary>
        /// Indicates whether we should write the human-readable file with the best theorems.
        /// </summary>
        public bool WriteReadableBestTheoremFile { get; }

        /// <summary>
        /// The path to the JSON file where the best theorems should be written to.
        /// </summary>
        public string JsonBestTheoremFilePath { get; }

        /// <summary>
        /// Indicates whether we should write the JSON file with the best theorems.
        /// </summary>
        public bool WriteJsonBestTheoremFile { get; }

        /// <summary>
        /// Indicates whether we should write the best theorem files every time we have a new best theorem.
        /// Otherwise they will be written when the generation is finished.
        /// </summary>
        public bool WriteBestTheoremsContinuously { get; }

        /// <summary>
        /// The path to the file with the usages of the inference rules used in theorem proofs.
        /// </summary>
        public string InferenceRuleUsageFilePath { get; }

        /// <summary>
        /// Indicates whether we should write the inference rule usage file.
        /// </summary>
        public bool WriteInferenceRuleUsages { get; }

        /// <summary>
        /// Indicates how often we log the number of generated configurations. If this number is 'n', then 
        /// after every n-th configuration there will be a message.
        /// </summary>
        public int ProgressLoggingFrequency { get; }

        /// <summary>
        /// Indicates whether we should log progress as described in <see cref="ProgressLoggingFrequency"/>.
        /// </summary>
        public bool LogProgress { get; }

        /// <summary>
        /// Indicates whether <see cref="IGeneratedProblemAnalyzer"/> should be called only for the
        /// configurations of the last iteration. Otherwise it is called for every generated configuration.
        /// </summary>
        public bool AnalyzeOnlyLastIteration { get; }

        /// <summary>
        /// Indicates whether <see cref="IBestTheoremFinder"/> should track at most one interesting theorem per
        /// configuration, the one with the highest ranking. Otherwise it tracks all of them.
        /// </summary>
        public bool TakeAtMostInterestingTheoremPerConfiguration { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the<see cref="ProblemGenerationRunnerSettings"/> class.
        /// </summary>
        /// <param name="readableOutputWithoutProofsFolder"><see cref="ReadableOutputWithoutProofsFolder"/></param>
        /// <param name="writeReadableOutputWithoutProofs"><see cref="WriteReadableOutputWithoutProofs"/></param>
        /// <param name="readableOutputWithProofsFolder"><see cref="ReadableOutputWithProofsFolder"/></param>
        /// <param name="writeReadableOutputWithProofs"><see cref="WriteReadableOutputWithProofs"/></param>
        /// <param name="jsonOutputFolder"><see cref="JsonOutputFolder"/></param>
        /// <param name="writeJsonOutput"><see cref="WriteJsonOutput"/></param>
        /// <param name="outputFilePrefix"><see cref="OutputFilePrefix"/></param>
        /// <param name="fileExtension"><see cref="FileExtension"/></param>
        /// <param name="readableBestTheoremFilePath"><see cref="ReadableBestTheoremFilePath"/></param>
        /// <param name="writeReadableBestTheoremFile"><see cref="WriteReadableBestTheoremFile"/></param>
        /// <param name="jsonBestTheoremFilePath"><see cref="JsonBestTheoremFilePath"/></param>
        /// <param name="writeJsonBestTheoremFile"><see cref="WriteJsonBestTheoremFile"/></param>
        /// <param name="writeBestTheoremsContinuously"><see cref="WriteBestTheoremsContinuously"/></param>
        /// <param name="inferenceRuleUsageFilePath"><see cref="InferenceRuleUsageFilePath"/></param>
        /// <param name="writeInferenceRuleUsages"><see cref="WriteInferenceRuleUsages"/></param>
        /// <param name="progressLoggingFrequency"><see cref="ProgressLoggingFrequency"/></param>
        /// <param name="logProgress"><see cref="LogProgress"/></param>
        /// <param name="analyzeOnlyLastIteration"><see cref="AnalyzeOnlyLastIteration"/></param>
        /// <param name="takeAtMostInterestingTheoremPerConfiguration"><see cref="TakeAtMostInterestingTheoremPerConfiguration"/></param>
        public ProblemGenerationRunnerSettings(string readableOutputWithoutProofsFolder,
                                               bool writeReadableOutputWithoutProofs,
                                               string readableOutputWithProofsFolder,
                                               bool writeReadableOutputWithProofs,
                                               string jsonOutputFolder,
                                               bool writeJsonOutput,
                                               string outputFilePrefix,
                                               string fileExtension,
                                               string readableBestTheoremFilePath,
                                               bool writeBestTheoremsContinuously,
                                               bool writeReadableBestTheoremFile,
                                               string jsonBestTheoremFilePath,
                                               bool writeJsonBestTheoremFile,
                                               string inferenceRuleUsageFilePath,
                                               bool writeInferenceRuleUsages,
                                               int progressLoggingFrequency,
                                               bool logProgress,
                                               bool analyzeOnlyLastIteration,
                                               bool takeAtMostInterestingTheoremPerConfiguration)
        {
            ReadableOutputWithoutProofsFolder = readableOutputWithoutProofsFolder;
            WriteReadableOutputWithoutProofs = writeReadableOutputWithoutProofs;
            ReadableOutputWithProofsFolder = readableOutputWithProofsFolder;
            WriteReadableOutputWithProofs = writeReadableOutputWithProofs;
            JsonOutputFolder = jsonOutputFolder;
            WriteJsonOutput = writeJsonOutput;
            OutputFilePrefix = outputFilePrefix;
            FileExtension = fileExtension;
            ReadableBestTheoremFilePath = readableBestTheoremFilePath;
            WriteReadableBestTheoremFile = writeReadableBestTheoremFile;
            JsonBestTheoremFilePath = jsonBestTheoremFilePath;
            WriteJsonBestTheoremFile = writeJsonBestTheoremFile;
            WriteBestTheoremsContinuously = writeBestTheoremsContinuously;
            InferenceRuleUsageFilePath = inferenceRuleUsageFilePath;
            WriteInferenceRuleUsages = writeInferenceRuleUsages;
            ProgressLoggingFrequency = progressLoggingFrequency;
            LogProgress = logProgress;
            AnalyzeOnlyLastIteration = analyzeOnlyLastIteration;
            TakeAtMostInterestingTheoremPerConfiguration = takeAtMostInterestingTheoremPerConfiguration;

            // Ensure that the output folder without proofs is set if we are supposed to use it
            if (writeReadableOutputWithoutProofs && readableOutputWithoutProofsFolder == null)
                throw new ArgumentException("The output folder without proofs must be set as we are going to use it.");

            // Ensure that the output folder with proofs is set if we are supposed to use it
            if (writeReadableOutputWithProofs && readableOutputWithProofsFolder == null)
                throw new ArgumentException("The output folder with proofs must be set as we are going to use it.");

            // Ensure that the JSON output folder with proofs is set if we are supposed to use it
            if (writeJsonOutput && jsonOutputFolder == null)
                throw new ArgumentException("The JSON output folder must be set as we are going to use it.");

            // Ensure that the readable best theorem file path is set if we are supposed to use it
            if (writeReadableBestTheoremFile && readableBestTheoremFilePath == null)
                throw new ArgumentException("The readable best theorem file path must be set as we are going to use it.");

            // Ensure that the JSON best theorem file path is set if we are supposed to use it
            if (writeJsonBestTheoremFile && jsonBestTheoremFilePath == null)
                throw new ArgumentException("The JSON best theorem file path must be set as we are going to use it.");

            // Ensure that the inference rule usage file path is set if we are supposed to use it
            if (writeInferenceRuleUsages && inferenceRuleUsageFilePath == null)
                throw new ArgumentException("The inference rule file path must be set as we are going to use it.");

            // Ensure the progress logging frequency is positive 
            if (logProgress && progressLoggingFrequency <= 0)
                throw new ArgumentOutOfRangeException(nameof(progressLoggingFrequency), "The progress logging frequency must be at least 1");

            // Ensure the file extension is set if we are supposed to use it
            if (writeReadableOutputWithoutProofs || writeReadableOutputWithProofs || fileExtension == null)
                throw new ArgumentException("The file extension must be set as we are going to write as least one type of file with it.");

            // Ensure the output file prefix is set if we are supposed to use it
            if (writeReadableOutputWithoutProofs || writeReadableOutputWithProofs || writeJsonOutput || outputFilePrefix == null)
                throw new ArgumentException("The output file prefix must be set as we are going to write as least one type of file with it.");
        }

        #endregion
    }
}
