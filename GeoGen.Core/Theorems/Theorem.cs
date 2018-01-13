using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a theorem that can be true in a configuration. For example:
    /// Collinear points, concyclic points, a line tangent to a circle. 
    /// </summary>
    public class Theorem
    {
        #region Public properties

        /// <summary>
        /// Gets the type of the theorem.
        /// </summary>
        public TheoremType Type { get; }

        /// <summary>
        /// Gets the list of theorem objects that this theorem is about.
        /// </summary>
        public IReadOnlyList<TheoremObject> InvolvedObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">The theorem type.</param>
        /// <param name="involvedObjects">The involved objects list.</param>
        public Theorem(TheoremType type, IReadOnlyList<TheoremObject> involvedObjects)
        {
            Type = type;
            InvolvedObjects = involvedObjects ?? throw new ArgumentNullException(nameof(involvedObjects));
        }

        #endregion
    }
}