using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The <see cref="IObjectConstructor"/> for <see cref="PredefinedConstructionType.SecondIntersectionOfTwoCircumcircles"/>>.
    /// </summary>
    public class SecondIntersectionOfTwoCircumcirclesConstructor : PredefinedConstructorBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SecondIntersectionOfTwoCircumcirclesConstructor"/> class.
        /// </summary>
        /// <param name="tracer">The tracer for unexpected analytic exceptions.</param>
        public SecondIntersectionOfTwoCircumcirclesConstructor(IConstructorFailureTracer tracer)
            : base(tracer)
        {
        }

        #endregion

        #region Construct implementation

        /// <inheritdoc/>
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

        #endregion
    }
}
