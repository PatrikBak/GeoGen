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
    internal class InternalAngelBisectorFromPointsConstructor : IPredefinedConstructor
    {
        public Type PredefinedConstructionType { get; } = typeof(InternalAngelBisectorFromPoints);

        public ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
        {
            if (constructedObjects == null)
                throw new ArgumentNullException(nameof(constructedObjects));

            try
            {
                ThrowHelper.ThrowExceptionIfNotTrue(constructedObjects.Count == 1);

                var constructedObject = constructedObjects[0];
                var arguments = constructedObject.PassedArguments;

                ThrowHelper.ThrowExceptionIfNotTrue(arguments.Count == 2);

                var rayIntersection = ((ObjectConstructionArgument) arguments[0]).PassedObject;

                var rayPoints = ((SetConstructionArgument) arguments[1]).PassedArguments
                        .Cast<ObjectConstructionArgument>()
                        .Select(arg => arg.PassedObject)
                        .ToList();

                List<IAnalyticalObject> ConstructorFunction(IObjectsContainer container)
                {
                    if (container == null)
                        throw new ArgumentNullException(nameof(container));

                    var point1 = container.Get<Point>(rayPoints[0]);
                    var point2 = container.Get<Point>(rayPoints[1]);

                    var intersection = container.Get<Point>(rayIntersection);

                    try
                    {
                        return new List<IAnalyticalObject> {intersection.InternalAngelBisector(point1, point2)};
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
            catch (Exception e)
            {
                throw new AnalyzerException("Incorrect arguments");
            }
        }
    }
}