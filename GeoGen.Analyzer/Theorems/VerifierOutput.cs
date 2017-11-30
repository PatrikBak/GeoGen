using System;
using System.Collections.Generic;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal sealed class VerifierOutput
    {
        public TheoremType TheoremType { get; set; }

        public Func<IObjectsContainer, bool> VerifierFunction { get; set; }

        public ConfigurationObjectsMap AllObjects { get; set; }

        public List<GeometricalObject> InvoldedObjects { get; set; }
    }
}