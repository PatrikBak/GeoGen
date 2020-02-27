namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// The settings for <see cref="LineTangentToCircleTheoremFinder"/>.
    /// </summary>
    public class LineTangentToCircleTheoremFinderSettings
    {
        #region Public properties

        /// <summary>
        /// Indicates whether we should take find also tangency of a line and circle 
        /// for which the tangent point is one of the points of the picture.
        /// </summary>
        public bool ExcludeTangencyInsidePicture { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LineTangentToCircleTheoremFinderSettings"/> class.
        /// </summary>
        /// <param name="excludeTangencyInsidePicture">Indicates whether we should take find also tangency of a line and circle 
        /// for which the tangent point is one of the points of the picture.</param>
        public LineTangentToCircleTheoremFinderSettings(bool excludeTangencyInsidePicture)
        {
            ExcludeTangencyInsidePicture = excludeTangencyInsidePicture;
        }

        #endregion
    }
}
