using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;

namespace GeoGen.Core.Generator
{
    public sealed class GeneratorOutput
    {
        public Configuration Configuration { get; set; }

        public List<Theorem> Theorems { get; set; }
    }
}