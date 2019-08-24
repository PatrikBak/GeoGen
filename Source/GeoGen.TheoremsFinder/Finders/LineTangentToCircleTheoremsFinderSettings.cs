namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// The settings for <see cref="LineTangentToCircleTheoremsFinder"/>.
    /// </summary>
    public class LineTangentToCircleTheoremsFinderSettings
    {
        /// <summary>
        /// Indicated whether we should take find also tangency of a line and circle 
        /// for which the tangent point is one of the points of the picture.
        /// </summary>
        public bool ExcludeTangencyInsidePicture { get; set; }
    }
}
