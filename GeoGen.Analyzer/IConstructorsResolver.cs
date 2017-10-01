using GeoGen.Core.Constructions;

namespace GeoGen.Analyzer
{
    internal interface IConstructorsResolver
    {
        IPredefinedConstructor Resolve(PredefinedConstruction predefinedConstruction);
    }
}