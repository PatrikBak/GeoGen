using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a constructor of <see cref="Theorem"/> objects.
    /// </summary>
    internal interface ITheoremConstructor
    {
        /// <summary>
        /// Constructs a theorem.
        /// </summary>
        /// <param name="involvedObjects">The list of objects that this theorem is about.</param>
        /// <param name="type">The type of the theorem.</param>
        /// <returns>The theorem.</returns>
        Theorem Construct(List<GeometricalObject> involvedObjects, TheoremType type);
    }
}