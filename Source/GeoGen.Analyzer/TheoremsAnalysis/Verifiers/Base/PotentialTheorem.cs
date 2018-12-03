using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a wrapper for potential theorem that might hold a true. This 
    /// class represents an output from <see cref="ITheoremVerifier"/>.
    /// </summary>
    internal class PotentialTheorem
    {
        /// <summary>
        /// Gets or sets the type of this possible theorem.
        /// </summary>
        public TheoremType TheoremType { get; set; }

        /// <summary>
        /// Gets or sets the enumerable of the involved geometrical objects in this theorem.
        /// </summary>
        public IEnumerable<GeometricalObject> InvolvedObjects { get; set; }

        /// <summary>
        /// Gets or sets the verifier function that verifies this theorem
        /// in a given container. If the theorem is certainly true, then this
        /// function should be null.
        /// </summary>
        public Func<IObjectsContainer, bool> VerifierFunction { get; set; }
    }
}