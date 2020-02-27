namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// Represents all types of things that we can draw
    /// </summary>
    public enum DrawingCommandType
    {
        /// <summary>
        /// The point mark.
        /// </summary>
        Point,

        /// <summary>
        /// The segment between two points (the order of points is not important).
        /// </summary>
        Segment,

        /// <summary>
        /// The segment between two ordered points A, B such that it is slightly extended beyond B.
        /// </summary>
        ShiftedSegment,

        /// <summary>
        /// The line, which might optionally contain points that should it pass through when drawn.
        /// </summary>
        Line,

        /// <summary>
        /// The line, that has to contain at least points that should it pass through when drawn, drawn 
        /// such that it is slightly extended beyond all these points.
        /// </summary>
        ShiftedLine,

        /// <summary>
        /// The circle.
        /// </summary>
        Circle
    }
}
