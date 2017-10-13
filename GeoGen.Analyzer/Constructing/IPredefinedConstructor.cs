using System;

namespace GeoGen.Analyzer.Constructing
{
    internal interface IPredefinedConstructor : IObjectsConstructor
    {
        Type PredefinedConstructionType { get; }
    }
}
