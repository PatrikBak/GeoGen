namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// Represents the style of how we want to draw a certain object. 
    /// </summary>
    public enum ObjectDrawingStyle
    {
        /// <summary>
        /// Represents that an object should be drawn as an auxiliary one (for example, with thin lines).
        /// </summary>
        AuxiliaryObject = 0,

        /// <summary>
        /// Represents that an object should be drawn normally (for example, with the default line style).
        /// </summary>
        NormalObject = 1,

        /// <summary>
        /// Represents that an object should be marked as an object that is a part of a theorem (for example, colorfully).
        /// </summary>
        TheoremObject = 2
    }
}
