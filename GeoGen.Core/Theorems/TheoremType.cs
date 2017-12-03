namespace GeoGen.Core.Theorems
{
    /// <summary>
    /// Represents a type of a <see cref="Theorem"/>, such as collinearity.
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
        /// Three concurrent objects that each is either line, or circle
        /// </summary>
        ConcurrentObjects,

        /// <summary>
        /// Two configuration objects are geometrically the same
        /// </summary>
        SameObjects
    }
}