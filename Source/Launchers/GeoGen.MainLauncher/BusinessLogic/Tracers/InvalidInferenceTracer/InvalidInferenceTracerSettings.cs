using System;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The settings for <see cref="InvalidInferenceTracer"/>.
    /// </summary>
    public class InvalidInferenceTracerSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the file where the info about failures is written to.
        /// </summary>
        public string FailureFilePath { get; }

        /// <summary>
        /// Indicates whether we should log the failures using the log system.
        /// </summary>
        public bool LogFailures { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInferenceTracerSettings"/> class.
        /// </summary>
        /// <param name="failureFilePath">The path to the file where the info about failures is written to.</param>
        /// <param name="logFailures">Indicates whether we should log the failures using the log system.</param>
        public InvalidInferenceTracerSettings(string failureFilePath, bool logFailures)
        {
            FailureFilePath = failureFilePath ?? throw new ArgumentNullException(nameof(failureFilePath));
            LogFailures = logFailures;
        }

        #endregion
    }
}
