namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// The settings for <see cref="TangentCirclesTheoremsFinder"/>.
    /// </summary>
    public class TangentCirclesTheoremsFinderSettings
    {
        /// <summary>
        /// Indicated whether we should take find also tangency of circles 
        /// for which the tangent point is one of the points of the picture.
        /// </summary>
        public bool ExcludeTangencyInsidePicture { get; set; }
    }
}
