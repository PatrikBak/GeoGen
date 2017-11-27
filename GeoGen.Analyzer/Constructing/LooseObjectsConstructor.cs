using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.AnalyticalGeometry.RandomObjects;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Constructing
{
    internal class LooseObjectsConstructor : ILooseObjectsConstructor
    {
        private readonly IRandomObjectsProvider _provider;

        public LooseObjectsConstructor(IRandomObjectsProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public List<IAnalyticalObject> Construct(IEnumerable<LooseConfigurationObject> looseObjects)
        {
            if (looseObjects == null)
                throw new ArgumentNullException(nameof(looseObjects));

            return looseObjects.Select
            (
                looseObject =>
                {
                    if (looseObject == null)
                        throw new ArgumentException("Loose objects contains null value.");

                    switch (looseObject.ObjectType)
                    {
                        case ConfigurationObjectType.Point:
                            return _provider.NextRandomObject<Point>();
                        case ConfigurationObjectType.Line:
                            return _provider.NextRandomObject<Line>();
                        case ConfigurationObjectType.Circle:
                            return _provider.NextRandomObject<Circle>();
                        default:
                            throw new AnalyzerException("Unhandled case.");
                    }
                }
            ).ToList();
        }
    }
}