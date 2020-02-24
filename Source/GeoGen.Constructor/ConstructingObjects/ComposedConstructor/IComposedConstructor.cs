using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents an <see cref="IObjectConstructor"/> that is able to create <see cref="ConstructedConfigurationObject"/>
    /// that are defined using a <see cref="ComposedConstruction"/>.
    /// </summary>
    public interface IComposedConstructor : IObjectConstructor
    {
    }
}