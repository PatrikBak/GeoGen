namespace GeoGen.Constructor
{
    /// <summary>
    /// An <see cref="InconsistentPicturesException"/> thrown when concyclities between
    /// the passed points are not the same in every picture.
    /// </summary>
    public class InconsistentConcyclityException : InconsistentPicturesException
    {
        #region Public properties

        /// <summary>
        /// The points that whose concyclity couldn't be determined consistently.
        /// </summary>
        public IReadOnlyList<PointObject> ProblematicPoints { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentConcyclityException"/> class.
        /// </summary>
        /// <param name="problematicPoints">The points that whose concyclity couldn't be determined consistently.</param>
        public InconsistentConcyclityException(IReadOnlyList<PointObject> problematicPoints)
        {
            ProblematicPoints = problematicPoints ?? throw new ArgumentNullException(nameof(problematicPoints));
        }

        #endregion
    }
}