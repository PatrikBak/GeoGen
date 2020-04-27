using System;

namespace GeoGen.TheoremProver.ObjectIntroductionRuleProvider
{
    /// <summary>
    /// The settings for <see cref="ObjectIntroductionRuleProvider"/>
    /// </summary>
    public class ObjectIntroductionRuleProviderSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the file with object introduction rules.
        /// </summary>
        public string FilePath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIntroductionRuleProviderSettings"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file with object introduction rules.</param>
        public ObjectIntroductionRuleProviderSettings(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        #endregion
    }
}
