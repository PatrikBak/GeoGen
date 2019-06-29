using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles"/>>.
    /// </summary>
    public class SecondIntersectionOfTwoCircumcirclesConstructor : PredefinedConstructorBase
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
            var E = (Point)input[4];

            // Make sure the points that should make circles are not collinear
            if (AnalyticHelpers.AreCollinear(A, B, C) || AnalyticHelpers.AreCollinear(A, D, E))
                return null;

            // Now we can create the circles
            var circleABC = new Circle(A, B, C);
            var circleADE = new Circle(A, D, E);

            // Let's intersect them and take the intersection different from the common point
            var intersections = circleABC.IntersectWith(circleADE).Where(intersection => intersection != A).ToArray();

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
