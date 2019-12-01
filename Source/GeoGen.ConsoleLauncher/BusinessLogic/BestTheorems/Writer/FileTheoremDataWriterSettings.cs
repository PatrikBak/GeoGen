using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="FileTheoremDataWriter"/>.
    /// </summary>
    public class FileTheoremDataWriterSettings
    {
        #region Public properties

        /// <summary>
        /// The file where the theorems will be written to. 
        /// </summary>
        public string TheoremFilePath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTheoremDataWriter"/> class.
        /// </summary>
        /// <param name="theoremFilePath">The file where the theorems will be written to. </param>
        public FileTheoremDataWriterSettings(string theoremFilePath)
        {
            TheoremFilePath = theoremFilePath ?? throw new ArgumentNullException(nameof(theoremFilePath));
        }

        #endregion
    }
}