using GeoGen.Core.Constructions;

namespace GeoGen.Analyzer.Constructing
{
    internal interface IConstructorsResolver
    {
        IPredefinedConstructor Resolve(PredefinedConstruction predefinedConstruction);
    }
}