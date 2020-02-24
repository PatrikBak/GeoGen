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
        /// The settings for <see cref="TypeRanker"/>. It can be null if this aspect is not ranked.
        /// </summary>
        public TypeRankerSettings TypeRankerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRankingSettings"/> class.
        /// </summary>
        /// <param name="theoremRankerSettings">The settings for <see cref="TheoremRanker.TheoremRanker"/>.</param>
        /// <param name="typeRankerSettings">The settings for <see cref="TypeRanker"/>. It can be null if this aspect is not ranked.</param>
        public TheoremRankingSettings(TheoremRankerSettings theoremRankerSettings, TypeRankerSettings typeRankerSettings)
        {
            TheoremRankerSettings = theoremRankerSettings ?? throw new ArgumentNullException(nameof(theoremRankerSettings));
            TypeRankerSettings = typeRankerSettings;

            // Ensure that type ranker settings are set if this type is ranked
            if (theoremRankerSettings.RankingCoefficients.ContainsKey(RankedAspect.Type) && typeRankerSettings == null)
                throw new ArgumentException("The type ranker must have its settings set as this aspect is ranked.");
        }

        #endregion
    }
}
