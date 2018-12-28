using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a geometric theorem that holds true for some objects. It is defined by 
    /// its <see cref="TheoremType"/> and <see cref="TheoremObject"/>s that wrap the actual
    /// <see cref="ConfigurationObject"/>s. The number of theorems objects, as well as their
    /// <see cref="TheoremObjectSignature"/>, depends on the type of the theorem.
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
        /// Initializes a new instance of the <see cref="Theorem"/> object.
        /// </summary>
        /// <param name="type">The type of the theorem.</param>
        /// <param name="involvedObjects">The list of theorem objects that this theorem is about.</param>
        public Theorem(TheoremType type, IReadOnlyList<TheoremObject> involvedObjects)
        {
            Type = type;
            InvolvedObjects = involvedObjects ?? throw new ArgumentNullException(nameof(involvedObjects));
        }

        #endregion
    }
}