using GeoGen.TheoremRanker;
using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings related to the theorem ranker module.
    /// </summary>
    public class TheoremRankingSettings
    {
        #region Public properties

        /// <summary>
        /// The settings for <see cref="TheoremRanker.TheoremRanker"/>.
        /// </summary>
        public TheoremRankerSettings TheoremRankerSettings { get; }

        /// <summary>
        /// The settings for <see cref="TypeRanker"/>.
        /// </summary>
        public TypeRankerSettings TypeRankerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRankingSettings"/> class.
        /// </summary>
        /// <param name="theoremRankerSettings">The settings for <see cref="TheoremRanker.TheoremRanker"/>.</param>
        /// <param name="typeRankerSettings">The settings for <see cref="TypeRanker"/>.</param>
        public TheoremRankingSettings(TheoremRankerSettings theoremRankerSettings, TypeRankerSettings typeRankerSettings)
        {
            TheoremRankerSettings = theoremRankerSettings ?? throw new ArgumentNullException(nameof(theoremRankerSettings));
            TypeRankerSettings = typeRankerSettings ?? throw new ArgumentNullException(nameof(typeRankerSettings));
        }

        #endregion
    }
}
