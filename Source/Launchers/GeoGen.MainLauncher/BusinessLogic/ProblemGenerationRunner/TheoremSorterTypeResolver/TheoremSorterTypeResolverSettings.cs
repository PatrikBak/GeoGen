using System;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The settings for <see cref="TheoremSorterTypeResolver"/>.
    /// </summary>
    public class TheoremSorterTypeResolverSettings
    {
        #region Public properties

        /// <summary>
        /// Gets the number of theorems that are supposed to be tracked per each theorem type.
        /// </summary>
        public int MaximalTrackedTheoremsPerType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSorterTypeResolverSettings"/> class.
        /// </summary>
        /// <param name="maximalTrackedTheoremsPerType"><inheritdoc cref="MaximalTrackedTheoremsPerType" path="/summary"/></param>
        public TheoremSorterTypeResolverSettings(int maximalTrackedTheoremsPerType)
        {
            MaximalTrackedTheoremsPerType = maximalTrackedTheoremsPerType;

            // Ensure the maximal count is positive
            if (maximalTrackedTheoremsPerType <= 0)
                throw new ArgumentOutOfRangeException(nameof(maximalTrackedTheoremsPerType), maximalTrackedTheoremsPerType, "The maximal number of tracked theorems per type must be positive.");
        }

        #endregion
    }
}