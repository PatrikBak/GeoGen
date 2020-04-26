using GeoGen.TheoremSorter;
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
        /// <see cref="ReadableOutputWithoutProofsFolder"/>, <see cref="ReadableOutputWithProofsFolder"/>
        /// and <see cref="ReadableBestTheoremFolder"/>.
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// The folder with human-readable best theorem files where the best theorems of individual types should be written.
        /// </summary>
        public string ReadableBestTheoremFolder { get; }

        /// <summary>
        /// Indicates whether we should write human-readable best theorem files to <see cref="ReadableBestTheoremFolder"/>.
        /// </summary>
        public bool WriteReadableBestTheorems { get; }

        /// <summary>
        /// The folder with JOSN best theorem files where the best theorems of individual types should be written.
        /// </summary>
        public string JsonBestTheoremFolder { get; }

        /// <summary>
        /// Indicates whether we should write the JSON file with the best theorems.
        /// </summary>
        public bool WriteJsonBestTheorems { get; }

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
        /// Indicates whether <see cref="ITheoremSorter"/> should track at most one interesting theorem per
        /// configuration, the one with the highest ranking. Otherwise it tracks all of them.
        /// </summary>
        public bool TakeAtMostOneInterestingTheoremPerConfiguration { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the<see cref="ProblemGenerationRunnerSettings"/> class.
        /// </summary>
        /// <param name="readableOutputWithoutProofsFolder"><inheritdoc cref="ReadableOutputWithoutProofsFolder" path="/summary"/></param>
        /// <param name="writeReadableOutputWithoutProofs"><inheritdoc cref="WriteReadableOutputWithoutProofs" path="/summary"/></param>
        /// <param name="readableOutputWithProofsFolder"><inheritdoc cref="ReadableOutputWithProofsFolder" path="/summary"/></param>
        /// <param name="writeReadableOutputWithProofs"><inheritdoc cref="WriteReadableOutputWithProofs" path="/summary"/></param>
        /// <param name="jsonOutputFolder"><inheritdoc cref="JsonOutputFolder" path="/summary"/></param>
        /// <param name="writeJsonOutput"><inheritdoc cref="WriteJsonOutput" path="/summary"/></param>
        /// <param name="outputFilePrefix"><inheritdoc cref="OutputFilePrefix" path="/summary"/></param>
        /// <param name="fileExtension"><inheritdoc cref="FileExtension" path="/summary"/></param>
        /// <param name="readableBestTheoremFolder"><inheritdoc cref="ReadableBestTheoremFolder" path="/summary"/></param>
        /// <param name="writeReadableBestTheorems"><inheritdoc cref="WriteReadableBestTheorems" path="/summary"/></param>
        /// <param name="jsonBestTheoremFolder"><inheritdoc cref="JsonBestTheoremFolder" path="/summary"/></param>
        /// <param name="writeJsonBestTheorems"><inheritdoc cref="WriteJsonBestTheorems" path="/summary"/></param>
        /// <param name="writeBestTheoremsContinuously"><inheritdoc cref="WriteBestTheoremsContinuously" path="/summary"/></param>
        /// <param name="inferenceRuleUsageFilePath"><inheritdoc cref="InferenceRuleUsageFilePath" path="/summary"/></param>
        /// <param name="writeInferenceRuleUsages"><inheritdoc cref="WriteInferenceRuleUsages" path="/summary"/></param>
        /// <param name="progressLoggingFrequency"><inheritdoc cref="ProgressLoggingFrequency" path="/summary"/></param>
        /// <param name="logProgress"><inheritdoc cref="LogProgress"/></param>
        /// <param name="takeAtMostOneInterestingTheoremPerConfiguration"><inheritdoc cref="TakeAtMostOneInterestingTheoremPerConfiguration"/></param>
        public ProblemGenerationRunnerSettings(string readableOutputWithoutProofsFolder,
                                               bool writeReadableOutputWithoutProofs,
                                               string readableOutputWithProofsFolder,
                                               bool writeReadableOutputWithProofs,
                                               string jsonOutputFolder,
                                               bool writeJsonOutput,
                                               string outputFilePrefix,
                                               string fileExtension,
                                               string readableBestTheoremFolder,
                                               bool writeReadableBestTheorems,
                                               string jsonBestTheoremFolder,
                                               bool writeJsonBestTheorems,
                                               bool writeBestTheoremsContinuously,
                                               string inferenceRuleUsageFilePath,
                                               bool writeInferenceRuleUsages,
                                               int progressLoggingFrequency,
                                               bool logProgress,
                                               bool takeAtMostOneInterestingTheoremPerConfiguration)
        {
            ReadableOutputWithoutProofsFolder = readableOutputWithoutProofsFolder;
            WriteReadableOutputWithoutProofs = writeReadableOutputWithoutProofs;
            ReadableOutputWithProofsFolder = readableOutputWithProofsFolder;
            WriteReadableOutputWithProofs = writeReadableOutputWithProofs;
            JsonOutputFolder = jsonOutputFolder;
            WriteJsonOutput = writeJsonOutput;
            OutputFilePrefix = outputFilePrefix;
            FileExtension = fileExtension;
            ReadableBestTheoremFolder = readableBestTheoremFolder;
            WriteReadableBestTheorems = writeReadableBestTheorems;
            JsonBestTheoremFolder = jsonBestTheoremFolder;
            WriteJsonBestTheorems = writeJsonBestTheorems;
            WriteBestTheoremsContinuously = writeBestTheoremsContinuously;
            InferenceRuleUsageFilePath = inferenceRuleUsageFilePath;
            WriteInferenceRuleUsages = writeInferenceRuleUsages;
            ProgressLoggingFrequency = progressLoggingFrequency;
            LogProgress = logProgress;
            TakeAtMostOneInterestingTheoremPerConfiguration = takeAtMostOneInterestingTheoremPerConfiguration;

            // Ensure that the output folder without proofs is set if we are supposed to use it
            if (writeReadableOutputWithoutProofs && readableOutputWithoutProofsFolder == null)
                throw new MainLauncherException("The output folder without proofs must be set as we are going to use it.");

            // Ensure that the output folder with proofs is set if we are supposed to use it
            if (writeReadableOutputWithProofs && readableOutputWithProofsFolder == null)
                throw new MainLauncherException("The output folder with proofs must be set as we are going to use it.");

            // Ensure that the JSON output folder with proofs is set if we are supposed to use it
            if (writeJsonOutput && jsonOutputFolder == null)
                throw new MainLauncherException("The JSON output folder must be set as we are going to use it.");

            // Ensure that the readable best theorem folder is set if we are supposed to use it
            if (writeReadableBestTheorems && readableBestTheoremFolder == null)
                throw new MainLauncherException("The readable best theorem folder must be set as we are going to use it.");

            // Ensure that the JSON best theorem folder is set if we are supposed to use it
            if (writeJsonBestTheorems && jsonBestTheoremFolder == null)
                throw new MainLauncherException("The JSON best theorem folder must be set as we are going to use it.");

            // Ensure that the inference rule usage file path is set if we are supposed to use it
            if (writeInferenceRuleUsages && inferenceRuleUsageFilePath == null)
                throw new MainLauncherException("The inference rule file path must be set as we are going to use it.");

            // Ensure the progress logging frequency is positive 
            if (logProgress && progressLoggingFrequency <= 0)
                throw new ArgumentOutOfRangeException(nameof(progressLoggingFrequency), "The progress logging frequency must be at least 1");

            // Ensure the file extension is set if we are supposed to use it
            if ((writeReadableOutputWithoutProofs || writeReadableOutputWithProofs) && fileExtension == null)
                throw new MainLauncherException("The file extension must be set as we are going to write as least one type of file with it.");

            // Ensure the output file prefix is set if we are supposed to use it
            if ((writeReadableOutputWithoutProofs || writeReadableOutputWithProofs || writeJsonOutput) && outputFilePrefix == null)
                throw new MainLauncherException("The output file prefix must be set as we are going to write as least one type of file with it.");
        }

        #endregion
    }
}