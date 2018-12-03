namespace GeoGen.Core
{
    /// <summary>
    /// Represents a type of a signature of a <see cref="TheoremObject"/>.
    /// </summary>
    public enum TheoremObjectSignature
    {
        /// <summary>
        /// Represents a object that is defined explicitly (by the
        /// <see cref="ConfigurationObject"/> that represents it). 
        /// </summary>
        SingleObject,

        /// <summary>
        /// Represents a line that is implicitly defined by point
        /// <see cref="ConfigurationObject"/>s.
        /// </summary>
        LineGivenByPoints,

        /// <summary>
        /// Represents a circle that is implicitly defined by point
        /// <see cref="ConfigurationObject"/>s.
        /// </summary>
        CircleGivenByPoints
    }
}