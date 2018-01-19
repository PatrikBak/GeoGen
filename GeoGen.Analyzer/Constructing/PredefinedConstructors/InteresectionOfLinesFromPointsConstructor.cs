using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A constructor for the <see cref="IntersectionFromPoints"/> construction.
    /// </summary>
    internal class IntersectionOfLinesFromPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Constructs a given list of constructed configurations objects. This objects 
        /// should be the result of the same construction.
        /// </summary>
        /// <param name="constructedObjects">The constructed objects list.</param>
        /// <returns>The constructor output.</returns>
        public override ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
        {
            var constructedObject = constructedObjects[0];
            var arguments = constructedObject.PassedArguments;

            var setArguments = ((SetConstructionArgument) arguments[0]).PassedArguments.ToList();

            var passedPoints1 = ((SetConstructionArgument) setArguments[0]).PassedArguments.ToList();
            var passedPoints2 = ((SetConstructionArgument) setArguments[1]).PassedArguments.ToList();

            var obj1 = ((ObjectConstructionArgument) passedPoints1[0]).PassedObject;
            var obj2 = ((ObjectConstructionArgument) passedPoints1[1]).PassedObject;
            var obj3 = ((ObjectConstructionArgument) passedPoints2[0]).PassedObject;
            var obj4 = ((ObjectConstructionArgument) passedPoints2[1]).PassedObject;

            // Prepare the constructor function
            List<AnalyticalObject> ConstructorFunction(IObjectsContainer container)
            {
                var point1 = container.Get<Point>(obj1);
                var point2 = container.Get<Point>(obj2);
                var point3 = container.Get<Point>(obj3);
                var point4 = container.Get<Point>(obj4);

                try
                {
                    // Create the set of our points
                    var points = new HashSet<Point> {point1, point2, point3, point4};

                    // Create lines. This might throw an AnalyticalException if the points are same 
                    var line1 = new Line(point1, point2);
                    var line2 = new Line(point3, point4);

                    // If the lines are fine, intersect them
                    var result = line1.IntersectionWith(line2);

                    // If there is no intersection, or the intersection is the same as some 
                    // of our points (which shouldn't be because of the contract of this constructor)
                    if (result == null || points.Contains(result))
                        return null;

                    // Otherwise the point is correct, we can return it wrapped in a list
                    return new List<AnalyticalObject> {result};
                }
                catch (AnalyticalException)
                {
                    // If we got here, then we have either equal points, or equal lines
                    // In that case, the construction has failed
                    return null;
                }
            }

            // Construct the theorems list
            var theorems = new List<Theorem>
            {
                // The intersection is collinear with first two points
                new Theorem(TheoremType.CollinearPoints, new List<TheoremObject>
                {
                    new TheoremObject(obj1),
                    new TheoremObject(obj2),
                    new TheoremObject(constructedObject)
                }),
                // As well as with the other two points
                new Theorem(TheoremType.CollinearPoints, new List<TheoremObject>
                {
                    new TheoremObject(obj3),
                    new TheoremObject(obj4),
                    new TheoremObject(constructedObject)
                })
            };

            // Now we can finally construct the output
            return new ConstructorOutput
            {
                ConstructorFunction = ConstructorFunction,
                Theorems = theorems
            };
        }
    }
}