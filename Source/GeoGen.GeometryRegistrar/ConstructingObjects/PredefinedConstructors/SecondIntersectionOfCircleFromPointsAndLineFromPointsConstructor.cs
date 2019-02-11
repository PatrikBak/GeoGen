using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Linq;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.SecondIntersectionOfCircleFromPointsAndLineFromPoints"/>>.
    /// </summary>
    public class SecondIntersectionOfCircleFromPointsAndLineFromPointsConstructor : PredefinedConstructorBase
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

            // According to the definition of the construction, the last three
            // points make the circle. First we make sure they're not collinear
            if (AnalyticHelpers.AreCollinear(point2, point3, point4))
                return null;

            // Now we can create the circle
            var circle = new Circle(point2, point3, point4);

            // The line is given by the first two points
            var line = new Line(point1, point2);

            // Let's intersect them and take the intersection different from the second point
            var intersections = circle.IntersectWith(line).Where(intersection => intersection != point2).ToArray();

            // If there is no such intersection, then the line is probably tangent to this circle
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