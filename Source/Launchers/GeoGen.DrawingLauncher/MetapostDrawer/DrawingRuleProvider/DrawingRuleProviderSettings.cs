namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// The settings for <see cref="DrawingRuleProvider"/>
    /// </summary>
    public class DrawingRuleProviderSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the file with drawing rules.
        /// </summary>
        public string FilePath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingRuleProviderSettings"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file with drawing rules.</param>
        public DrawingRuleProviderSettings(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        #endregion
    }
}
