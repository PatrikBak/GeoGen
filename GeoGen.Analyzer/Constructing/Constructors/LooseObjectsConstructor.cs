using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Constructing.Constructors
{
    internal class LooseObjectsConstructor : ILooseObjectsConstructor
    {
        private readonly IRandomObjectsProvider _provider;

        public List<AnalyticalObject> Construct(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            return looseObjects.Select
            (
                looseObject =>
                {
                    AnalyticalObject result;

                    switch (looseObject.ObjectType)
                    {
                        case ConfigurationObjectType.Point:
                            result = _provider.NextRandomObject<Point>();
                            break;
                        case ConfigurationObjectType.Line:
                            result = _provider.NextRandomObject<Line>();
                            break;
                        case ConfigurationObjectType.Circle:
                            result = _provider.NextRandomObject<Circle>();
                            break;
                        default:
                            throw new AnalyzerException("Unhandled case.");
                    }

                    return result;
                }
            ).ToList();
        }
    }
}