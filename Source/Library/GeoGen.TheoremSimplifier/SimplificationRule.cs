using GeoGen.Core;
using System;

namespace GeoGen.TheoremSimplifier
{
    /// <summary>
    /// Represents a rule that can be used to simplify <see cref="TheoremObject"/>s. For example: 
    /// If <see cref="SimplifableObject"/> = LineObject(A, Incenter(A, B, C)), then this object can 
    /// be simplified to <see cref="SimplifiedObject"/> = LineObject(InternalAngleBisector(A, B, C)).
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

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRule"/> class.
        /// </summary>
        /// <param name="simplifableObject">The template of an object that can be simplified.</param>
        /// <param name="simplifiedObject">The template of the simplification of <see cref="SimplifiedObject"/>.</param>
        public SimplificationRule(TheoremObject simplifableObject, TheoremObject simplifiedObject)
        {
            SimplifableObject = simplifableObject ?? throw new ArgumentNullException(nameof(simplifableObject));
            SimplifiedObject = simplifiedObject ?? throw new ArgumentNullException(nameof(simplifiedObject));
        }

        #endregion
    }
}