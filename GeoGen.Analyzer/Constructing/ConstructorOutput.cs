using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Constructing
{
    internal class ConstructorOutput
    {
        public Func<IObjectsContainer, List<AnalyticalObject>> ConstructorFunction { get; set; }

        public List<Theorem> Theorems { get; set; }
    }
}
