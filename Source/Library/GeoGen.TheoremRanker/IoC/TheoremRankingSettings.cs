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
        /// The settings for <see cref="TheoremTypeRanker"/>. It can be null if this aspect is not ranked.
        /// </summary>
        public TheoremTypeRankerSettings TheoremTypeRankerSettings { get; }

        /// <summary>
        /// The settings for <see cref="ConstructionsRanker"/>. It can be null if this aspect is not ranked.
        /// </summary>
        public ConstructionsRankerSettings ConstructionsRankerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRankingSettings"/> class.
        /// </summary>
        /// <param name="theoremRankerSettings">The settings for <see cref="TheoremRanker"/>.</param>
        /// <param name="theoremTypeRankerSettings">The settings for <see cref="TheoremTypeRanker"/>. It can be null if this aspect is not ranked.</param>
        /// <param name="constructionsRankerSettings">The settings for <see cref="ConstructionsRanker"/>. It can be null if this aspect is not ranked.</param>
        public TheoremRankingSettings(TheoremRankerSettings theoremRankerSettings,
                                      TheoremTypeRankerSettings theoremTypeRankerSettings,
                                      ConstructionsRankerSettings constructionsRankerSettings)
        {
            TheoremRankerSettings = theoremRankerSettings ?? throw new ArgumentNullException(nameof(theoremRankerSettings));
            TheoremTypeRankerSettings = theoremTypeRankerSettings;
            ConstructionsRankerSettings = constructionsRankerSettings;

            // Ensure that theorem type ranker settings are set if this aspect is ranked
            if (theoremRankerSettings.RankingCoefficients.ContainsKey(RankedAspect.TheoremType) && theoremTypeRankerSettings == null)
                throw new ArgumentException("The theorem type ranker must have its settings set as this aspect is ranked.");

            // Ensure that constructions ranker settings are set if this aspect is ranked
            if (theoremRankerSettings.RankingCoefficients.ContainsKey(RankedAspect.Constructions) && constructionsRankerSettings == null)
                throw new ArgumentException("The constructions ranker must have its settings set as this aspect is ranked.");
        }

        #endregion
    }
}
