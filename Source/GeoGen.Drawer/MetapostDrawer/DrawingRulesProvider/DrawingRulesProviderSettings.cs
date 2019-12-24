using System;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The settings for <see cref="DrawingRulesProvider"/>
    /// </summary>
    public class DrawingRulesProviderSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the file with drawing rules.
        /// </summary>
        public string FilePath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingRulesProviderSettings"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file with drawing rules.</param>
        public DrawingRulesProviderSettings(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        #endregion
    }
}
