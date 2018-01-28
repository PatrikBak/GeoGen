using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an output of <see cref="ITheoremVerifier"/>s.
    /// </summary>
    internal class VerifierOutput
    {
        /// <summary>
        /// Gets the type of the theorem.
        /// </summary>
        public TheoremType Type { get; set; }

        /// <summary>
        /// Gets or sets the verifier function that verifies this theorem
        /// in a given container.
        /// </summary>
        public Func<IObjectsContainer, bool> VerifierFunction { get; set; }

        /// <summary>
        /// Gets or sets the theorem that are certainly true regardless of a 
        /// contextual container that we put it against (for example: the 
        /// collinearity theorem could be determined from the contextual
        /// container itself because the actual analytical geometry is handled in it).
        /// </summary>
        public bool AlwaysTrue { get; set; }

        /// <summary>
        /// Gets or sets the list of geometrical objects that this theorem is about.
        /// </summary>
        public List<GeometricalObject> InvoldedObjects { get; set; }
    }
}