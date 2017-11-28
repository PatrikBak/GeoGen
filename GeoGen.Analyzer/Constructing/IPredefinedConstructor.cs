using System;
using GeoGen.Core.Constructions.PredefinedConstructions;

namespace GeoGen.Analyzer.Constructing
{
    /// <summary>
    /// Represents an <see cref="IObjectsConstructor"/> of predefined constructions, 
    /// such as <see cref="Midpoint"/>.
    /// </summary>
    internal interface IPredefinedConstructor : IObjectsConstructor
    {
        /// <summary>
        /// Gets the type of this predefined construction.
        /// </summary>
        Type PredefinedConstructionType { get; }
    }
}