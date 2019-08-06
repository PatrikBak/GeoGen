using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.SecondIntersectionOfCircleAndLineFromPoints"/>>.
    /// </summary>
    public class SecondIntersectionOfCircleAndLineFromPointsConstructor : PredefinedConstructorBase
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
            var A = (Point)input[0];
            var B = (Point)input[1];
            var C = (Point)input[2];
            var D = (Point)input[3];

            // Make sure A, C, D makes a circle according to the definition of the construction
            if (AnalyticHelpers.AreCollinear(A, C, D))
                return null;

            // Now we can create the circle
            var circleACD = new Circle(A, C, D);

            // Create the line
            var lineAB = new Line(A, B);

            // Let's intersect them and take the intersection different from the common point
            var intersections = circleACD.IntersectWith(lineAB).Where(intersection => intersection != A).ToArray();

            // If there is no such intersection, then the line is probably tangent to this circle
            if (intersections.Length == 0)
                return null;

            // If there are two such intersections, then the precision system has really failed...
            if (intersections.Length == 2)
                return null;

            // Otherwise we can take the only intersection as the result
            return intersections[0];
        }
    }
}