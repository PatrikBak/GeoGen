namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a reason based on which a theorem have been attempted to be proven / proven.
    /// </summary>
    public enum DerivationRule
    {
        /// <summary>
        /// The theorem is true in the configuration that does not contain the last object
        /// of the examined configuration, i.e. the theorem's proof is not interesting at this point.
        /// </summary>
        TrueInPreviousConfiguration,

        /// <summary>
        /// The theorem can be stated in a configuration that does not contain every object
        /// of the current configuration. 
        /// 
        /// NOTE: The difference between this rule and <see cref="TrueInPreviousConfiguration"/> 
        ///       is that in this case the smaller configuration might not have been examined or
        ///       explicitly defined.
        /// </summary>
        DefinableSimpler,

        /// <summary>
        /// The theorem follows from the general mathematical transitivity rule of some relations.
        /// This rule is clearly used for binary relations, for example the relation '=', or parallelity.
        /// It is also used for collinearity and line concurrency (which are trinary relations), 
        /// concyclity (quaternary relation) and also in a combination with incidence and equality
        /// (if a=b and a lies on c, then b lies on c; if a=b and c lies of a, then c lies on b).
        /// </summary>
        Transitivity,

        /// <summary>
        /// The theorem is a consequence of a simpler (template) theorem that is presumed to be true.
        /// Such theorems might, but don't have to have assumptions that are needed for them to be true.
        /// </summary>
        Subtheorem,

        /// <summary>
        /// The theorem that can be derived directly from the construction of the last object
        /// of the examined configuration. 
        /// 
        /// NOTE: This rule can be viewed as a concrete case of the <see cref="Subtheorem"/> rule.
        ///       Since the used template theorem is really trivial and has no needed assumptions,
        ///       it is more convenient to declare as a separate rule.
        /// </summary>
        TrivialTheorem,

        /// <summary>
        /// If we have 4 concyclic points and the center of the circle they make, then 3 right 
        /// equal segment theorems imply the concyclity, and the concyclity implies the equal segment theorems.
        /// </summary>
        ConcyclicPointsWithExplicitCenter,

        /// <summary>
        /// If we have three lines a, b, c such that (a, b) and (a, c) are perpendicular and (b, c)
        /// are parallel, then any two of these statements imply the last one.
        /// </summary>
        PerpendicularLineToParallelLines
    }
}
