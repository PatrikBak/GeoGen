using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Constructing.Constructors.PredefinedConstructors
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

                List<GeometricalObject> ConstructorFunction(IObjectsContainer container)
                {
                    var point1 = container.Get<Point>(obj1);
                    var point2 = container.Get<Point>(obj2);

                    var result = AnalyticalHelpers.Midpoint(point1, point2);

                    if (result == null)
                        return null;

                    result.ConfigurationObject = constructedObject;

                    return new List<GeometricalObject> {result};
                }

                var objects = new List<TheoremObject>
                {
                    new SingleTheoremObject(obj1),
                    new SingleTheoremObject(obj2),
                    new SingleTheoremObject(constructedObject)
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