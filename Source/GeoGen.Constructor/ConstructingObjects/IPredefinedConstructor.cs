using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents an <see cref="IObjectConstructor"/> of a <see cref="PredefinedConstruction"/> 
    /// given by its <see cref="PredefinedConstructionType"/>.
    /// </summary>
    public interface IPredefinedConstructor : IObjectConstructor
    {
        /// <summary>
        /// Gets the type of the predefined construction that this constructor performs.
        /// </summary>
        PredefinedConstructionType Type { get; }
    }
}