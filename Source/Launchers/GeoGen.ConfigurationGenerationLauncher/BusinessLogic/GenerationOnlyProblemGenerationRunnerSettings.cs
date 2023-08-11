namespace GeoGen.ConfigurationGenerationLauncher
{
    /// <summary>
    /// The settings for <see cref="GenerationOnlyProblemGenerationRunner"/>.
    /// </summary>
    public class GenerationOnlyProblemGenerationRunnerSettings
    {
        #region Public properties

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
        /// The way in which configurations are counted in while generating.
        /// </summary>
        public CountingMode CountingMode { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationOnlyProblemGenerationRunnerSettings"/> class.
        /// </summary>
        /// <param name="generationProgresLoggingFrequency">Indicates how often we log the number of generated configurations. If this number is 'n', then after every n-th configuration there will be a message.</param>
        /// <param name="logProgress">Indicates whether we should log the progress.</param>
        /// <param name="countingMode">The way in which configurations are counted in while generating.</param>
        public GenerationOnlyProblemGenerationRunnerSettings(int generationProgresLoggingFrequency, bool logProgress, CountingMode countingMode)
        {
            GenerationProgresLoggingFrequency = generationProgresLoggingFrequency;
            LogProgress = logProgress;
            CountingMode = countingMode;

            // Ensure the frequency is positive
            if (logProgress && generationProgresLoggingFrequency <= 0)
                throw new ArgumentOutOfRangeException(nameof(generationProgresLoggingFrequency), "The generation logging frequency must be at least 1");
        }

        #endregion
    }
}