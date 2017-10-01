using System;
using System.Collections.Generic;
using GeoGen.Analyzer.Geometry;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Analyzer
{
    internal interface IPredefinedConstructor
    {
        Type PredefinedConstructionType { get; }

        GeometricalObject Apply(IReadOnlyList<ConstructionArgument> arguments);
    }
}