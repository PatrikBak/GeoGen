using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Core;
using GeoGen.Utilities.Helpers;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A constructor for the <see cref="IntersectionFromPoints"/> construction.
    /// </summary>
    internal sealed class IntersectionOfLinesFromPointsConstructor : PredefinedConstructorBase
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
                ThrowHelper.ThrowExceptionIfNotTrue(constructedObjects.Count == 1);

                var constructedObject = constructedObjects[0];
                var arguments = constructedObject.PassedArguments;

                ThrowHelper.ThrowExceptionIfNotTrue(arguments.Count == 1);

                var setArguments = ((SetConstructionArgument) arguments[0]).PassedArguments.ToList();

                ThrowHelper.ThrowExceptionIfNotTrue(setArguments.Count == 2);

                var passedPoints1 = ((SetConstructionArgument) setArguments[0]).PassedArguments.ToList();
                var passedPoints2 = ((SetConstructionArgument) setArguments[1]).PassedArguments.ToList();

                ThrowHelper.ThrowExceptionIfNotTrue(passedPoints1.Count == 2);
                ThrowHelper.ThrowExceptionIfNotTrue(passedPoints2.Count == 2);

                var obj1 = ((ObjectConstructionArgument) passedPoints1[0]).PassedObject;
                var obj2 = ((ObjectConstructionArgument) passedPoints1[1]).PassedObject;
                var obj3 = ((ObjectConstructionArgument) passedPoints2[0]).PassedObject;
                var obj4 = ((ObjectConstructionArgument) passedPoints2[1]).PassedObject;

                ThrowHelper.ThrowExceptionIfNotTrue(obj1.ObjectType == ConfigurationObjectType.Point);
                ThrowHelper.ThrowExceptionIfNotTrue(obj2.ObjectType == ConfigurationObjectType.Point);
                ThrowHelper.ThrowExceptionIfNotTrue(obj3.ObjectType == ConfigurationObjectType.Point);
                ThrowHelper.ThrowExceptionIfNotTrue(obj4.ObjectType == ConfigurationObjectType.Point);

                List<IAnalyticalObject> ConstructorFunction(IObjectsContainer container)
                {
                    if (container == null)
                        throw new ArgumentNullException(nameof(container));

                    var point1 = container.Get<Point>(obj1);
                    var point2 = container.Get<Point>(obj2);
                    var point3 = container.Get<Point>(obj3);
                    var point4 = container.Get<Point>(obj4);

                    var points = new HashSet<Point> {point1, point2, point3, point4};

                    if (point1 == point2 || point3 == point4)
                        return null;

                    var line1 = new Line(point1, point2);
                    var line2 = new Line(point3, point4);

                    Point result;

                    try
                    {
                        result = line1.IntersectionWith(line2);
                    }
                    catch (ArgumentException)
                    {
                        return null;
                    }

                    if (result == null || points.Contains(result))
                        return null;

                    return new List<IAnalyticalObject> {result};
                }

                var objects1 = new List<TheoremObject>
                {
                    new TheoremObject(obj1),
                    new TheoremObject(obj2),
                    new TheoremObject(constructedObject)
                };
                var objects2 = new List<TheoremObject>
                {
                    new TheoremObject(obj3),
                    new TheoremObject(obj4),
                    new TheoremObject(constructedObject)
                };

                var collinearityTheorem1 = new Theorem(TheoremType.CollinearPoints, objects1);
                var collinearityTheorem2 = new Theorem(TheoremType.CollinearPoints, objects2);

                return new ConstructorOutput
                {
                    ConstructorFunction = ConstructorFunction,
                    Theorems = new List<Theorem> {collinearityTheorem1, collinearityTheorem2}
                };
            }
            catch (Exception)
            {
                throw new AnalyzerException("Incorrect arguments.");
            }
        }
    }
}