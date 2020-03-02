using System;
using System.Collections.Generic;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The settings for <see cref="ConstructionsRanker"/>.
    /// </summary>
    public class ConstructionsRankerSettings
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping construction names to rankings.
        /// </summary>
        public IReadOnlyDictionary<string, double> ConstructionRankings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructionsRankerSettings"/>.
        /// </summary>
        /// <param name="constructionRankings">The dictionary mapping construction names to rankings.</param>
        public ConstructionsRankerSettings(IReadOnlyDictionary<string, double> constructionRankings)
        {
            ConstructionRankings = constructionRankings ?? throw new ArgumentNullException(nameof(constructionRankings));
        }

        #endregion
    }
}
