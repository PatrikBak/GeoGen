using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a wrapper of a <see cref="Core.Theorem"/> together with the <see cref="Core.Configuration"/>
    /// where it holds and its <see cref="TheoremRanking"/>.
    /// </summary>
    public class RankedTheorem
    {
        #region Public properties

        /// <summary>
        /// The actual theorem that was ranked.
        /// </summary>
        public Theorem Theorem { get; }

        /// <summary>
        /// The ranking of the <see cref="Theorem"/> with respect to the configuration where it was discovered.
        /// </summary>
        public TheoremRanking Ranking { get; }

        /// <summary>
        /// The configuration where the <see cref="Theorem"/> holds.
        /// </summary>
        public Configuration Configuration { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RankedTheorem"/> class.
        /// </summary>
        /// <param name="theorem">The actual theorem that was ranked.</param>
        /// <param name="ranking">The ranking of the <see cref="Theorem"/> with respect to the configuration where it was discovered.</param>
        /// <param name="configuration">The configuration where the <see cref="Theorem"/> holds.</param>
        public RankedTheorem(Theorem theorem, TheoremRanking ranking, Configuration configuration)
        {
            Theorem = theorem ?? throw new ArgumentNullException(nameof(theorem));
            Ranking = ranking ?? throw new ArgumentNullException(nameof(ranking));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion
    }
}