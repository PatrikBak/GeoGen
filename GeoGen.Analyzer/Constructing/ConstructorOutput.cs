using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Constructing
{
    class ConstructorOutput
    {
        public List<GeometricalObject> Objects { get; set; }

        public List<Theorem> Theorems { get; set; }
    }
}
