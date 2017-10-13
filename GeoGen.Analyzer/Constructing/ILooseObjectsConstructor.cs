using System.Collections.Generic;
using GeoGen.AnalyticalGeometry.Objects;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Constructing
{
    internal interface ILooseObjectsConstructor
    {
        List<AnalyticalObject> Construct(IEnumerable<LooseConfigurationObject> looseObjects);
    }
}
