using System;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The settings related to the theorem ranker module.
    /// </summary>
    public class TheoremRankingSettings
    {
        #region Public properties

        /// <summary>
        /// The settings for <see cref="TheoremRanker"/>.
        /// </summary>
        public TheoremRankerSettings TheoremRankerSettings { get; }

        /// <summary>
        /// The settings for <see cref="SpecificConstructionsRanker"/>. It can be null if this aspect is not ranked.
        /// </summary>
        public SpecificConstructionsRankerSettings SpecificConstructionsRankerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRankingSettings"/> class.
        /// </summary>
        /// <param name="theoremRankerSettings">The settings for <see cref="TheoremRanker"/>.</param>
        /// <param name="specificConstructionsRankerSettings">The settings for <see cref="SpecificConstructionsRanker"/>. It can be null if this aspect is not ranked.</param>
        public TheoremRankingSettings(TheoremRankerSettings theoremRankerSettings,
                                      SpecificConstructionsRankerSettings specificConstructionsRankerSettings)
        {
            TheoremRankerSettings = theoremRankerSettings ?? throw new ArgumentNullException(nameof(theoremRankerSettings));
            SpecificConstructionsRankerSettings = specificConstructionsRankerSettings;

            // Ensure that constructions ranker settings are set if this aspect is ranked
            if (theoremRankerSettings.RankingCoefficients.ContainsKey(RankedAspect.SpecificConstructions) && specificConstructionsRankerSettings == null)
                throw new TheoremRankerException("The constructions ranker must have its settings set as this aspect is ranked.");
        }

        #endregion
    }
}
