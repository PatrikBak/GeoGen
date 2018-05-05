using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents an <see cref="IObjectsConstructor"/> that is able to construct objects
    /// created by <see cref="ComposedConstruction"/>s
    /// </summary>
    internal interface IComposedConstructor : IObjectsConstructor
    {
    }
}