using GeoGen.Core;
using System;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremRanker"/> that simply merges the results of provided
    /// <see cref="IAspectTheoremRanker"/>s.
    /// </summary>
    public class TheoremRanker : ITheoremRanker
    {
        #region Private properties

        /// <summary>
        /// The settings for the ranker.
        /// </summary>
        private readonly TheoremRankerSettings _settings;

        /// <summary>
        /// The rankers for particular aspects.
        /// </summary>
        private readonly IAspectTheoremRanker[] _rankers;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRanker"/> class.
        /// </summary>
        /// <param name="settings">The settings for the ranker.</param>
        /// <param name="rankers">The rankers for particular aspects.</param>
        public TheoremRanker(TheoremRankerSettings settings, IAspectTheoremRanker[] rankers)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _rankers = rankers ?? throw new ArgumentNullException(nameof(rankers));
        }

        #endregion

        #region ITheoremRanker implementation

        /// <inheritdoc/>
        public TheoremRanking Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
        {
            // Prepare the ranking dictionary by applying every ranker
            var ranking = _rankers.Select(ranker => (ranker.RankedAspect, ranking: ranker.Rank(theorem, configuration, allTheorems)))
                // And wrapping the result to a dictionary together with the coefficient from the settings
                .ToDictionary(pair => pair.RankedAspect, pair => new RankingData(pair.ranking, _settings.RankingCoefficients[pair.RankedAspect]));

            // Wrap the final ranking in a ranking object
            return new TheoremRanking(ranking);
        }

        #endregion
    }
}
