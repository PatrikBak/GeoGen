using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.Objects;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Constructing
{
    internal interface ILooseObjectsConstructor
    {
        List<IAnalyticalObject> Construct(IEnumerable<LooseConfigurationObject> looseObjects);
    }
}
