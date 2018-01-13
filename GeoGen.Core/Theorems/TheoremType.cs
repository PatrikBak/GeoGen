namespace GeoGen.Core
{
    /// <summary>
    /// Represents a type of a <see cref="Theorem"/>.
    /// </summary>
    public enum TheoremType
    {
        /// <summary>
        /// Three or more points are collinear
        /// </summary>
        CollinearPoints,

        /// <summary>
        /// Four or more points are concyclic
        /// </summary>
        ConcyclicPoints,

        /// <summary>
        /// Three or more concurrent objects that each is either line, or circle
        /// </summary>
        ConcurrentObjects
    }
}