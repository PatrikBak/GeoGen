using System;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    internal  class VerifierOutput
    {
        public Func<IObjectsContainer, bool> VerifierFunction { get; set; }

        public List<GeometricalObject> InvoldedObjects { get; set; }
    }
}