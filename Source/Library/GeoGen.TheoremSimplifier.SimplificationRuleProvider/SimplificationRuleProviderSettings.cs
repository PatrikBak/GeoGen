namespace GeoGen.TheoremSimplifier.SimplificationRuleProvider
{
    /// <summary>
    /// The settings for <see cref="SimplificationRuleProvider"/>
    /// </summary>
    public class SimplificationRuleProviderSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the file with simplification rules.
        /// </summary>
        public string FilePath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRuleProviderSettings"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file with simplification rules.</param>
        public SimplificationRuleProviderSettings(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        #endregion
    }
}
