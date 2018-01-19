using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal class PerpendicularLineFromPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Constructs a given list of constructed configurations objects. This objects 
        /// should be the result of the same construction.
        /// </summary>
        /// <param name="constructedObjects">The constructed objects list.</param>
        /// <returns>The constructor output.</returns>
        public override ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
        {
            if (constructedObjects == null)
                throw new ArgumentNullException(nameof(constructedObjects));

            try
            {
                
                var constructedObject = constructedObjects[0];
                var arguments = constructedObject.PassedArguments;

                var rayIntersection = ((ObjectConstructionArgument)arguments[0]).PassedObject;

                var linePoints = ((SetConstructionArgument)arguments[1]).PassedArguments
                        .Cast<ObjectConstructionArgument>()
                        .Select(arg => arg.PassedObject)
                        .ToList();

                List<AnalyticalObject> ConstructorFunction(IObjectsContainer container)
                {
                    if (container == null)
                        throw new ArgumentNullException(nameof(container));

                    var linePoint1 = container.Get<Point>(linePoints[0]);
                    var linePoint2 = container.Get<Point>(linePoints[1]);

                    var pointFrom = container.Get<Point>(rayIntersection);

                    try
                    {
                        var line = new Line(linePoint1, linePoint2);

                        var perpendicularLine = line.PerpendicularLine(pointFrom);

                        return new List<AnalyticalObject> { perpendicularLine };
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
                throw new AnalyzerException("Incorrect arguments");
            }
        }
    }
}
