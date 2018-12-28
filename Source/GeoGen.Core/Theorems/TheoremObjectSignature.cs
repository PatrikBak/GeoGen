namespace GeoGen.Core
{
    /// <summary>
    /// Represents what geometrical object the internal objects of a <see cref="TheoremObject"/>
    /// actually represent.
    /// </summary>
    public enum TheoremObjectSignature
    {
        /// <summary>
        /// Represents a object that is defined explicitly (by the <see cref="ConfigurationObject"/> that represents it). 
        /// </summary>
        SingleObject,

        /// <summary>
        /// Represents a line that is implicitly defined by (at least 2) points.
        /// </summary>
        LineGivenByPoints,

        /// <summary>
        /// Represents a circle that is implicitly defined by (at least 3) points.
        /// </summary>
        CircleGivenByPoints
    }
}