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
        /// The theorem that can be derived directly from the construction of the last object
        /// of the examined configuration. 
        /// 
        /// NOTE: This rule can be viewed as a concrete case of the <see cref="Subtheorem"/> rule.
        ///       Since the used template theorem is really trivial and has no needed assumptions,
        ///       it is more convenient to declare as a separate rule.
        /// </summary>
        TrivialTheorem,

        /// <summary>
        /// The theorem is a consequence of a simpler (template) theorem that is presumed to be true.
        /// Such theorems might, but don't have to have assumptions that are needed for them to be true.
        /// </summary>
        Subtheorem,

        /// <summary>
        /// The theorem can be reformulated using equalities to another theorem.
        /// </summary>
        ReformulatedTheorem,

        /// <summary>
        /// The theorem which connects collinearity of three points and some two theorems involving two lines. 
        /// For example, if we have theorems t1 = Parallel Lines [A, B], l, t2 = Parallel Lines [A, C], l
        /// and c = Collinear Points A, B, C, then (t1, c) => t2 and (t2, c) => t1. In the cases of
        /// Parallel Lines we can also say that (t1, t2) => c. This further implication holds true for 
        /// Parallel Lines, Perpendicular Lines and ConcurrentLines.
        /// </summary>
        CollinearityWithLinesFromPoints,

        /// <summary>
        /// If we have 4 concyclic points and the center of the circle they make, then 3 right 
        /// equal segment theorems imply the concyclity, and the concyclity with two equal segment
        /// theorems implies the third.
        /// </summary>
        ConcyclicPointsWithExplicitCenter,

        /// <summary>
        /// The theorem which connects some two theorems with a common line l, and two incidences that
        /// explicitly define this line. For example: If we have theorems t1 = Parallel Lines [A, B], l', 
        /// t2 = Parallel Lines l, [A, B], and two incidences i1 = Incidence A, l, i2 = Incidence B, l, 
        /// then (t1, i1, i2) => t2 and (t2, i1, i2) => t1. In the case of Parallel Lines we can also
        /// say (t1, t2, i1) => i2 and (t1, t2, i2) => i1. This further implication holds true for 
        /// Parallel Lines, Perpendicular Lines and ConcurrentLines.
        /// </summary>
        ExplicitLineWithIncidences,

        /// <summary>
        /// The theorem which connects 3 incidences and collinearity. Any three of these four theorems
        /// imply the fourth.
        /// </summary>
        IncidencesAndCollinearity,

        /// <summary>
        /// The theorem which connects 4 incidences and concyclity. Any four of these five theorems
        /// imply the fifth.
        /// </summary>
        IncidencesAndConcyclity,

        /// <summary>
        /// When we have theorems t1: AB || CD, t2: BC || AD, t3: AB = CD, t4: BC = AD, then it is true that 
        /// (t1, t2) => t3 and (t1, t2) => t4.
        /// </summary>
        Parallelogram,

        /// <summary>
        /// If we have three lines a, b, c such that (a, b) and (a, c) are perpendicular and (b, c)
        /// are parallel, then any two of these statements imply the last one.
        /// </summary>
        PerpendicularLineToParallelLines,

        /// <summary>
        /// If we have concyclic points (A, B, C, D), (C, D, E, F), (E, F, A, B), and concurrent lines 
        /// [A, B], [C, D], [E, F], then any three of these theorems imply the fourth.
        /// </summary>
        RadicalAxis,

        /// <summary>
        /// If we have theorems t1: AB || CD, t2: BC || AD, t3: AB ⟂ CD => 
        /// </summary>
        Rectangle,

        /// <summary>
        /// The theorem where we have fours points A, B, C, D, three theorems Perpendicular Lines BA, CA;
        /// Perpendicular Lines BD, CD; Concyclic Points A, B, C, D, then any two of these theorems 
        /// imply the third.
        /// </summary>
        ThalesTheorem,

        /// <summary>
        /// The theorem follows from the general mathematical transitivity rule of some relations.
        /// This rule is clearly used for binary relations, for example the relation '=', or parallelity.
        /// It is also used for collinearity and line concurrency (which are trinary relations), 
        /// concyclity (quaternary relation).
        /// </summary>
        Transitivity
    }
}
