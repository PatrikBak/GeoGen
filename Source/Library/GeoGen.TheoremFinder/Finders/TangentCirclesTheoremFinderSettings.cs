namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// The settings for <see cref="TangentCirclesTheoremFinder"/>.
    /// </summary>
    public class TangentCirclesTheoremFinderSettings
    {
        #region Public properties

        /// <summary>
        /// Indicates whether we should take find also tangency of circles 
        /// for which the tangent point is one of the points of the picture.
        /// </summary>
        public bool ExcludeTangencyInsidePicture { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TangentCirclesTheoremFinderSettings"/> class.
        /// </summary>
        /// <param name="excludeTangencyInsidePicture">Indicates whether we should take find also tangency of circles 
        /// for which the tangent point is one of the points of the picture.</param>
        public TangentCirclesTheoremFinderSettings(bool excludeTangencyInsidePicture)
        {
            ExcludeTangencyInsidePicture = excludeTangencyInsidePicture;
        }

        #endregion
    }
}
