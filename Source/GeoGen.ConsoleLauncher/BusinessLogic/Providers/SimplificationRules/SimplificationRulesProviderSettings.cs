using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="SimplificationRulesProvider"/>
    /// </summary>
    public class SimplificationRulesProviderSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the file with simplification rules.
        /// </summary>
        public string FilePath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRulesProviderSettings"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file with simplification rules.</param>
        public SimplificationRulesProviderSettings(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        #endregion
    }
}
