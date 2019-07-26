namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="AlgorithmRunner"/>
    /// </summary>
    public class AlgorithmRunnerSettings
    {
        /// <summary>
        /// Gets or sets how often we log the number of generated configurations.
        /// If this number is 'n', then after every n-th configuration there will be a message.
        /// </summary>
        public int GenerationProgresLoggingFrequency { get; set; }

        /// <summary>
        /// Gets or sets whether we should log the progress.
        /// </summary>
        public bool LogProgress { get; set; }
    }
}
