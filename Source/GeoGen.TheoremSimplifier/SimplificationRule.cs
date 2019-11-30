using GeoGen.Core;
using GeoGen.Utilities;
using System;

namespace GeoGen.TheoremSimplifier
{
    /// <summary>
    /// Represents a rule that can be used to simplify <see cref="TheoremObject"/>s. For example: 
    /// If <see cref="SimplifableObject"/> = LineObject(A, Incenter(A, B, C)), then this object can 
    /// be simplified to <see cref="SimplifiedObject"/> = LineObject(InternalAngleBisector(A, B, C)).
    /// Simplification might have <see cref="Assumptions"/>, for example [X, Y] can be simplified to
    /// PerpendicularBisector(A, B), if XA = XB and YA = YB.
    /// </summary>
    public class SimplificationRule
    {
        #region Public properties

        /// <summary>
        /// The template of an object that can be simplified.
        /// </summary>
        public TheoremObject SimplifableObject { get; }

        /// <summary>
        /// The template of the simplification of <see cref="SimplifiedObject"/>.
        /// </summary>
        public TheoremObject SimplifiedObject { get; }

        /// <summary>
        /// The assumptions needed for this simplification. There doesn't have to be any.
        /// </summary>
        public IReadOnlyHashSet<Theorem> Assumptions { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRule"/> class.
        /// </summary>
        /// <param name="simplifableObject">The template of an object that can be simplified.</param>
        /// <param name="simplifiedObject">The template of the simplification of <see cref="SimplifiedObject"/>.</param>
        /// <param name="assumptions">The assumptions needed for this simplification. There doesn't have to be any.</param>
        public SimplificationRule(TheoremObject simplifableObject, TheoremObject simplifiedObject, IReadOnlyHashSet<Theorem> assumptions)
        {
            SimplifableObject = simplifableObject ?? throw new ArgumentNullException(nameof(simplifableObject));
            SimplifiedObject = simplifiedObject ?? throw new ArgumentNullException(nameof(simplifiedObject));
            Assumptions = assumptions ?? throw new ArgumentNullException(nameof(assumptions));
        }

        #endregion
    }
}