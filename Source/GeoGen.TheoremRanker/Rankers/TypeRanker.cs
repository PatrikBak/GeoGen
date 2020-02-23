using GeoGen.Core;
using System;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Type"/>.
    /// </summary>
    public class TypeRanker : AspectTheoremRankerBase
    {
        #region Private fields

        /// <summary>
        /// The settings for the ranker.
        /// </summary>
        private readonly TypeRankerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRanker"/> class.
        /// </summary>
        /// <param name="settings">The settings for the ranker.</param>
        public TypeRanker(TypeRankerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region AspectTheoremRankerBase implementation

        /// <summary>
        /// Ranks a given theorem, potentially using all given provided context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <returns>A number representing the ranking of the theorem together with the explanation of how it was calculated.</returns>
        public override (double ranking, string message) Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
            // Simply return the ranking based on the settings with the message that it's from there
            => (_settings.TypeRankings[theorem.Type], "from settings");

        #endregion
    }
}
