using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="BestTheoremFinder"/>.
    /// </summary>
    public class BestTheoremFinderSettings
    {
        #region Public properties

        /// <summary>
        /// The maximal number of theorems that will be tracked.
        /// </summary>
        public int NumberOfTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BestTheoremFinderSettings"/> class.
        /// </summary>
        /// <param name="numberOfTheorems">The maximal number of theorems that will be tracked.</param>
        public BestTheoremFinderSettings(int numberOfTheorems)
        {
            NumberOfTheorems = numberOfTheorems;

            // Ensure the number of theorems is positive
            if (numberOfTheorems <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfTheorems), "The maximal number of theorem to be tracked must be at least 1");
        }

        #endregion
    }
}