using System;
using System.Collections.Generic;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects;

namespace GeoGen.Analyzer.Theorems
{
    internal sealed class VerifierOutput
    {
        public Func<IObjectsContainer, bool> VerifierFunction { get; set; }

        public List<GeometricalObject> InvoldedObjects { get; set; }
    }
}