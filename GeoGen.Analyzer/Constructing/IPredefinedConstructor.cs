using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an <see cref="IObjectsConstructor"/> of a <see cref="PredefinedConstruction"/>
    /// of some type.
    /// </summary>
    internal interface IPredefinedConstructor : IObjectsConstructor
    {
        /// <summary>
        /// Gets the type of predefined construction that this constructor performs.
        /// </summary>
        PredefinedConstructionType PredefinedConstructionType { get; }
    }
}