using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an <see cref="IObjectsConstructor"/> of a <see cref="PredefinedConstruction"/>
    /// of some <see cref="PredefinedConstructionType"/>.
    /// </summary>
    internal interface IPredefinedConstructor : IObjectsConstructor
    {
        /// <summary>
        /// Gets the type of the predefined construction that this constructor performs.
        /// </summary>
        PredefinedConstructionType Type { get; }
    }
}