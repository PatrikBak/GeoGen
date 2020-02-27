using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The settings for <see cref="TheoremTypeRanker"/>.
    /// </summary>
    public class TheoremTypeRankerSettings
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping theorem types to rankings.
        /// </summary>
        public IReadOnlyDictionary<TheoremType, double> TypeRankings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremTypeRankerSettings"/>.
        /// </summary>
        /// <param name="typeRankings">The dictionary mapping theorem types to rankings.</param>
        public TheoremTypeRankerSettings(IReadOnlyDictionary<TheoremType, double> typeRankings)
        {
            TypeRankings = typeRankings ?? throw new ArgumentNullException(nameof(typeRankings));
        }

        #endregion
    }
}
