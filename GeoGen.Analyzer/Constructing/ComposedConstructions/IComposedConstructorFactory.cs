using GeoGen.Core.Constructions;

namespace GeoGen.Analyzer
{
    internal interface IComposedConstructorFactory
    {
        IComposedConstructor Create(ComposedConstruction construction);
    }
}
