namespace GeoGen.Algorithm
{
    /// <summary>
    /// The settings for <see cref="BestTheoremsFinder"/>.
    /// </summary>
    public class BestTheoremsFinderSettings
    {
        #region Public properties

        /// <summary>
        /// The maximal number of theorems that will be tracked.
        /// </summary>
        public int NumberOfTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BestTheoremsFinderSettings"/> class.
        /// </summary>
        /// <param name="numberOfTheorems">The maximal number of theorems that will be tracked.</param>
        public BestTheoremsFinderSettings(int numberOfTheorems)
        {
            NumberOfTheorems = numberOfTheorems;
        }

        #endregion
    }
}