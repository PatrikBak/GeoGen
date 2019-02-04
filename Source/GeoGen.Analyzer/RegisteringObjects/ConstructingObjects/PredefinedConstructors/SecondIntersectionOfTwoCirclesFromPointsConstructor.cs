using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.SecondIntersectionOfTwoCirclesFromPoints"/>>.
    /// </summary>
    public class SecondIntersectionOfTwoCirclesFromPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override IAnalyticObject Construct(IAnalyticObject[] input)
        {
            // Pull the points
            var point1 = (Point) input[0];
            var point2 = (Point) input[1];
            var point3 = (Point) input[2];
            var point4 = (Point) input[3];
            var point5 = (Point) input[4];

            // Make sure the points that should make circles are not collinear
            if (AnalyticHelpers.AreCollinear(point1, point2, point3) || AnalyticHelpers.AreCollinear(point1, point4, point5))
                return null;

            // Now we can create the circles
            var circle1 = new Circle(point1, point2, point3);
            var circle2 = new Circle(point1, point4, point5);

            // Let's intersect them and take the intersection different from the first point
            var intersections = circle1.IntersectWith(circle2).Where(intersection => intersection != point1).ToArray();

            // If there is no such intersection, then the circles are probably tangent
            if (intersections.Length == 0)
                return null;

            // If there are two intersection, then the precision system has really failed...
            if (intersections.Length == 2)
                return null;

            // Otherwise we can take the only intersection as the result
            return intersections[0];
        }
    }
}
