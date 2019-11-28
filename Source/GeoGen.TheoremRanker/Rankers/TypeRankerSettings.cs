using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The settings for <see cref="TypeRanker"/>.
    /// </summary>
    public class TypeRankerSettings
    {
        #region Public properties

        /// <summary>
        /// The dictionary mapping theorem types to rankings.
        /// </summary>
        public IReadOnlyDictionary<TheoremType, double> TypeRankings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRankerSettings"/>.
        /// </summary>
        /// <param name="typeRankings">The dictionary mapping theorem types to rankings.</param>
        public TypeRankerSettings(IReadOnlyDictionary<TheoremType, double> typeRankings)
        {
            TypeRankings = typeRankings ?? throw new ArgumentNullException(nameof(typeRankings));
        }

        #endregion
    }
}
