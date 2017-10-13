using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry.Objects;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Constructing.PredefinedConstructors
{
    internal class MidpointConstructor : IPredefinedConstructor
    {
        public Type PredefinedConstructionType { get; } = typeof(Midpoint);

        public ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
        {
            try
            {
                var constructedObject = constructedObjects[0];
                var arguments = constructedObject.PassedArguments;

                var setArgument = (SetConstructionArgument) arguments[0];
                var passedPoints = setArgument.PassedArguments.ToList();

                var obj1 = ((ObjectConstructionArgument) passedPoints[0]).PassedObject;
                var obj2 = ((ObjectConstructionArgument) passedPoints[1]).PassedObject;

                List<AnalyticalObject> ConstructorFunction(IObjectsContainer container)
                {
                    var point1 = container.Get<Point>(obj1);
                    var point2 = container.Get<Point>(obj2);

                    var result = Point.Midpoint(point1, point2);

                    return result == null ? null : new List<AnalyticalObject> {result};
                }

                var objects = new List<TheoremObject>
                {
                    new TheoremObject(obj1),
                    new TheoremObject(obj2),
                    new TheoremObject(constructedObject)
                };
                var collinearityTheorem = new Theorem(TheoremType.CollinearPoints, objects);

                return new ConstructorOutput
                {
                    ConstructorFunction = ConstructorFunction,
                    Theorems = new List<Theorem> {collinearityTheorem}
                };
            }
            catch (Exception)
            {
                throw new AnalyzerException("Incorrect arguments.");
            }
        }
    }
}