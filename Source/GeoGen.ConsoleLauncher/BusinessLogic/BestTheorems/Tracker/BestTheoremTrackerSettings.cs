namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="BestTheoremTracker"/>.
    /// </summary>
    public class BestTheoremTrackerSettings
    {
        #region Public properties

        /// <summary>
        /// The maximal number of theorems that will be tracked.
        /// </summary>
        public int NumberOfTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BestTheoremTrackerSettings"/> class.
        /// </summary>
        /// <param name="numberOfTheorems">The maximal number of theorems that will be tracked.</param>
        public BestTheoremTrackerSettings(int numberOfTheorems)
        {
            NumberOfTheorems = numberOfTheorems;
        }

        #endregion
    }
}