using System;
using System.Collections.Generic;
using GeoGen.Utilities;

namespace GeoGen.Core.Theorems
{
    /// <summary>
    /// Represents a theorem that can hold true in a configuration. For example:
    /// Collinear points, concyclic points, a line tangent to a circle...
    /// </summary>
    public sealed class Theorem
    {
        #region Public properties

        /// <summary>
        /// Gets the type of the theorem.
        /// </summary>
        public TheoremType Type { get; }

        /// <summary>
        /// Gets the list of theorem objects that this theorem is about.
        /// </summary>
        public HashSet<TheoremObject> InvolvedObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new theorem given by a theorem type and a set of
        /// objects that this theorem is about.
        /// </summary>
        /// <param name="type">The theorem type.</param>
        /// <param name="involvedObjects">The involved objects list.</param>
        public Theorem(TheoremType type, HashSet<TheoremObject> involvedObjects)
        {
            Type = type;
            InvolvedObjects = involvedObjects ?? throw new ArgumentNullException(nameof(involvedObjects));

            if (InvolvedObjects.Empty())
                throw new ArgumentException("Involved objects can't be empty.");
        }

        #endregion
    }
}