using System;
using System.Collections.Generic;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Analyzer.Constructing
{
    internal interface IPredefinedConstructor
    {
        Type PredefinedConstructionType { get; }

        List<GeometricalObject> Apply(IReadOnlyList<ConstructionArgument> arguments, IObjectsContainer container);
    }
}