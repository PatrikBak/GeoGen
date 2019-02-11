using GeoGen.Core;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// Represents an <see cref="IObjectsConstructor"/> of a <see cref="PredefinedConstruction"/> 
    /// given by its <see cref="PredefinedConstructionType"/>.
    /// </summary>
    public interface IPredefinedConstructor : IObjectsConstructor
    {
        /// <summary>
        /// Gets the type of the predefined construction that this constructor performs.
        /// </summary>
        PredefinedConstructionType Type { get; }
    }
}