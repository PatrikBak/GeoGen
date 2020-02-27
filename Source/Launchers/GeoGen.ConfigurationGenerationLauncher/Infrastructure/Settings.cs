using GeoGen.ConfigurationGenerator;
using GeoGen.Infrastructure;
using GeoGen.MainLauncher;
using GeoGen.ProblemGenerator;
using System;

namespace GeoGen.ConfigurationGenerationLauncher
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
        /// The settings for <see cref="ProblemGeneratorInputProvider"/>.
        /// </summary>
        public ProblemGeneratorInputProviderSettings ProblemGeneratorInputProviderSettings { get; }

        /// <summary>
        /// The settings for <see cref="GenerationOnlyProblemGenerationRunner"/>.
        /// </summary>
        public GenerationOnlyProblemGenerationRunnerSettings GenerationOnlyProblemGenerationRunnerSettings { get; }

        /// <summary>
        /// The settings for <see cref="ProblemGenerator.ProblemGenerator"/>.
        /// </summary>
        public ProblemGeneratorSettings ProblemGeneratorSettings { get; }

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
        /// <param name="problemGeneratorInputProviderSettings">The settings for <see cref="ProblemGeneratorInputProvider"/>.</param>
        /// <param name="generationOnlyProblemGenerationRunnerSettings">The settings for <see cref="GenerationOnlyProblemGenerationRunner"/>.</param>
        /// <param name="problemGeneratorSettings">The settings for <see cref="ProblemGenerator.ProblemGenerator"/>.</param>
        /// <param name="generationSettings">The settings related to the generator module.</param>
        /// <param name="tracingSettings">The settings for all tracers.</param>
        public Settings(LoggingSettings loggingSettings,
                        ProblemGeneratorInputProviderSettings problemGeneratorInputProviderSettings,
                        GenerationOnlyProblemGenerationRunnerSettings generationOnlyProblemGenerationRunnerSettings,
                        ProblemGeneratorSettings problemGeneratorSettings,
                        GenerationSettings generationSettings,
                        TracingSettings tracingSettings)
        {
            LoggingSettings = loggingSettings ?? throw new ArgumentNullException(nameof(loggingSettings));
            ProblemGeneratorInputProviderSettings = problemGeneratorInputProviderSettings ?? throw new ArgumentNullException(nameof(problemGeneratorInputProviderSettings));
            GenerationOnlyProblemGenerationRunnerSettings = generationOnlyProblemGenerationRunnerSettings ?? throw new ArgumentNullException(nameof(generationOnlyProblemGenerationRunnerSettings));
            ProblemGeneratorSettings = problemGeneratorSettings ?? throw new ArgumentNullException(nameof(problemGeneratorSettings));
            GenerationSettings = generationSettings ?? throw new ArgumentNullException(nameof(generationSettings));
            TracingSettings = tracingSettings ?? throw new ArgumentNullException(nameof(tracingSettings));
        }

        #endregion
    }
}
