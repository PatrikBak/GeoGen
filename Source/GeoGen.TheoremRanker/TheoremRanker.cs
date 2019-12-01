using GeoGen.Core;
using GeoGen.TheoremProver;
using System;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The default implementation of <see cref="ITheoremRanker"/> that simply merges
    /// the results of provided <see cref="IAspectTheoremRanker"/>s.
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

        /// <summary>
        /// Ranks a given theorem, potentially using all given provided context.
        /// </summary>
        /// <param name="theorem">The theorem to be ranked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="allTheorems">The map of all theorems of the configuration.</param>
        /// <param name="proverOutput">The output from the theorem prover for all the theorems of the configuration.</param>
        /// <returns>The theorem ranking of the theorem containing ranking and coefficient for particular aspects of ranking.</returns>
        public TheoremRanking Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems, TheoremProverOutput proverOutput)
        {
            // Prepare the ranking dictionary by applying every ranker
            var ranking = _rankers.Select(ranker => (ranker.RankedAspect, rankerOutput: ranker.Rank(theorem, configuration, allTheorems, proverOutput)))
                // And wrapping the result to a dictionary together with the coefficient from the settings
                .ToDictionary(pair => pair.RankedAspect, pair => new RankingData(pair.rankerOutput.ranking, _settings.RankingCoefficients[pair.RankedAspect], pair.rankerOutput.message));

            // Wrap the final ranking in a ranking object
            return new TheoremRanking(ranking);
        }

        #endregion
    }
}
