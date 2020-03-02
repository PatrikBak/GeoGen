using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.TheoremType"/>.
    /// </summary>
    public class TheoremTypeRanker : AspectTheoremRankerBase
    {
        #region Private fields

        /// <summary>
        /// The settings for the ranker.
        /// </summary>
        private readonly TheoremTypeRankerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremTypeRanker"/> class.
        /// </summary>
        /// <param name="settings">The settings for the ranker.</param>
        public TheoremTypeRanker(TheoremTypeRankerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region AspectTheoremRankerBase implementation

        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
             // Get the ranking from the ranking dictionary from the settings, or use the default value of 0
             => _settings.TypeRankings.GetValueOrDefault(theorem.Type, 0d);

        #endregion
    }
}
