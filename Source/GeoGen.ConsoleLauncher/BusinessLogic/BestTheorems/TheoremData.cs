using GeoGen.Core;
using GeoGen.TheoremRanker;
using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a wrapper of a <see cref="Core.Theorem"/> together with its data.
    /// </summary>
    public class TheoremData
    {
        #region Public properties

        /// <summary>
        /// The theorem wrapped by this data object.
        /// </summary>
        public Theorem Theorem { get; }

        /// <summary>
        /// The configuration where the wrapped theorem holds.
        /// </summary>
        public Configuration Configuration { get; }

        /// <summary>
        /// The ranking of the theorem.
        /// </summary>
        public TheoremRanking Ranking { get; }

        /// <summary>
        /// The identifier of the theorem.
        /// </summary>
        public string Id { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremData"/> class.
        /// </summary>
        /// <param name="theorem">The theorem wrapped by this data object.</param>
        /// <param name="configuration">The configuration where the wrapped theorem holds.</param>
        /// <param name="ranking">The ranking of the theorem.</param>
        /// <param name="id">The identifier of the theorem.</param>
        public TheoremData(Theorem theorem, Configuration configuration, TheoremRanking ranking, string id)
        {
            Theorem = theorem ?? throw new ArgumentNullException(nameof(theorem));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Ranking = ranking ?? throw new ArgumentNullException(nameof(ranking));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        #endregion
    }
}