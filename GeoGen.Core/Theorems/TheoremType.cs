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
        /// Three or more lines are concurrent
        /// </summary>
        ConcurrentLines
    }
}