using System;

namespace GeoGen.Analyzer.Constructing.Constructors
{
    internal interface IPredefinedConstructor : IObjectsConstructor
    {
        Type PredefinedConstructionType { get; }
    }
}
