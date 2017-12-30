using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.PredefinedConstructions;
using GeoGen.Core.Theorems;
using GeoGen.Utilities.Helpers;

namespace GeoGen.Analyzer
{
    internal class CircumcircleFromPointsConstructor : IPredefinedConstructor
    {
        public Type PredefinedConstructionType { get; } = typeof(CircumcircleFromPoints);

        public ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
        {
            if (constructedObjects == null)
                throw new ArgumentNullException(nameof(constructedObjects));

            try
            {
                ThrowHelper.ThrowExceptionIfNotTrue(constructedObjects.Count == 1);

                var constructedObject = constructedObjects[0];
                var arguments = constructedObject.PassedArguments;

                ThrowHelper.ThrowExceptionIfNotTrue(arguments.Count == 1);

                var setArgument = (SetConstructionArgument) arguments[0];
                var passedPoints = setArgument.PassedArguments.ToList();

                ThrowHelper.ThrowExceptionIfNotTrue(passedPoints.Count == 3);

                var obj1 = ((ObjectConstructionArgument) passedPoints[0]).PassedObject;
                var obj2 = ((ObjectConstructionArgument) passedPoints[1]).PassedObject;
                var obj3 = ((ObjectConstructionArgument) passedPoints[2]).PassedObject;

                ThrowHelper.ThrowExceptionIfNotTrue(obj1.ObjectType == ConfigurationObjectType.Point);
                ThrowHelper.ThrowExceptionIfNotTrue(obj2.ObjectType == ConfigurationObjectType.Point);
                ThrowHelper.ThrowExceptionIfNotTrue(obj3.ObjectType == ConfigurationObjectType.Point);

                List<IAnalyticalObject> ConstructorFunction(IObjectsContainer container)
                {
                    if (container == null)
                        throw new ArgumentNullException(nameof(container));

                    var point1 = container.Get<Point>(obj1);
                    var point2 = container.Get<Point>(obj2);
                    var point3 = container.Get<Point>(obj3);

                    try
                    {
                        return new List<IAnalyticalObject> {new Circle(point1, point2, point3)};
                    }
                    catch (ArgumentException)
                    {
                        return null;
                    }
                }

                return new ConstructorOutput
                {
                    ConstructorFunction = ConstructorFunction,
                    Theorems = new List<Theorem>()
                };
            }
            catch (Exception)
            {
                throw new AnalyzerException("Incorrect arguments.");
            }
        }
    }
}