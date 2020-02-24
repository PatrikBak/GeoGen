using GeoGen.Algorithm;
using GeoGen.ConsoleLauncher;
using GeoGen.Generator;
using GeoGen.Infrastructure;
using System;

namespace GeoGen.GenerationLauncher
{
    /// <summary>
    /// Represents the settings of the application.
    /// </summary>
    public class Settings
    {
        #region Public properties

        /// <summary>
        /// The settings for the logging system.
        /// </summary>
        public LoggingSettings LoggingSettings { get; }

        /// <summary>
        /// The settings for <see cref="AlgorithmInputProvider"/>.
        /// </summary>
        public AlgorithmInputProviderSettings AlgorithmInputProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="GenerationAlgorithmRunner"/>.
        /// </summary>
        public GenerationAlgorithmRunnerSettings GenerationAlgorithmRunnerSettings { get; }

        /// <summary>
        /// The settings for <see cref="Algorithm.Algorithm"/>.
        /// </summary>
        public AlgorithmSettings AlgorithmSettings { get; }

        /// <summary>
        /// The settings related to the generator module.
        /// </summary>
        public GenerationSettings GenerationSettings { get; }

        /// <summary>
        /// The settings for all tracers.
        /// </summary>
        public TracingSettings TracingSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="loggingSettings">The settings for the logging system.</param>
        /// <param name="algorithmInputProviderSettings">The settings for <see cref="AlgorithmInputProvider"/>.</param>
        /// <param name="generationAlgorithmRunnerSettings">The settings for <see cref="GenerationAlgorithmRunner"/>.</param>
        /// <param name="algorithmSettings">The settings for <see cref="Algorithm.Algorithm"/>.</param>
        /// <param name="generationSettings">The settings related to the generator module.</param>
        /// <param name="tracingSettings">The settings for all tracers.</param>
        public Settings(LoggingSettings loggingSettings,
                        AlgorithmInputProviderSettings algorithmInputProviderSettings,
                        GenerationAlgorithmRunnerSettings generationAlgorithmRunnerSettings,
                        AlgorithmSettings algorithmSettings,
                        GenerationSettings generationSettings,
                        TracingSettings tracingSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            AlgorithmInputProviderSettings = algorithmInputProviderSettings ?? throw new ArgumentNullException(nameof(algorithmInputProviderSettings));
            GenerationAlgorithmRunnerSettings = generationAlgorithmRunnerSettings ?? throw new ArgumentNullException(nameof(generationAlgorithmRunnerSettings));
            AlgorithmSettings = algorithmSettings ?? throw new ArgumentNullException(nameof(algorithmSettings));
            GenerationSettings = generationSettings ?? throw new ArgumentNullException(nameof(generationSettings));
            TracingSettings = tracingSettings ?? throw new ArgumentNullException(nameof(tracingSettings));
        }

        #endregion
    }
}
