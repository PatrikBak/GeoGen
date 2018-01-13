using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal interface IComposedConstructorFactory
    {
        IComposedConstructor Create(ComposedConstruction construction);
    }
}
