namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// The settings for <see cref="LineTangentToCircleTheoremFinder"/>.
    /// </summary>
    public class LineTangentToCircleTheoremFinderSettings
    {
        /// <summary>
        /// Indicated whether we should take find also tangency of a line and circle 
        /// for which the tangent point is one of the points of the picture.
        /// </summary>
        public bool ExcludeTangencyInsidePicture { get; set; }
    }
}
