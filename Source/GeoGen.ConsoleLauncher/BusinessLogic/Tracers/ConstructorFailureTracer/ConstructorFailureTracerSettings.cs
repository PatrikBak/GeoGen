using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="ConstructorFailureTracer"/>.
    /// </summary>
    public class ConstructorFailureTracerSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the file where the info about failures is written to.
        /// </summary>
        public string FailuresFilePath { get; }

        /// <summary>
        /// Indicates whether we should log the failures using the log system.
        /// </summary>
        public bool LogFailures { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorFailureTracerSettings"/> class.
        /// </summary>
        /// <param name="failuresFilePath">The path to the file where the info about failures is written to.</param>
        /// <param name="logFailures">Indicates whether we should log the failures using the log system.</param>
        public ConstructorFailureTracerSettings(string failuresFilePath, bool logFailures)
        {
            FailuresFilePath = failuresFilePath ?? throw new ArgumentNullException(nameof(failuresFilePath));
            LogFailures = logFailures;
        }

        #endregion
    }
}
