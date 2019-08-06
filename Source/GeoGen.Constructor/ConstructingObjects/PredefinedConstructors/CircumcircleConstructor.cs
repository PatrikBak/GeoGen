using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.Circumcircle"/>>.
    /// </summary>
    public class CircumcircleConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override IAnalyticObject Construct(IAnalyticObject[] input)
        {
            // Get the points
            var A = (Point)input[0];
            var B = (Point)input[1];
            var C = (Point)input[2];

            // If the points are collinear, the construction can't be done
            if (AnalyticHelpers.AreCollinear(A, B, C))
                return null;

            // Otherwise construct the circumcircle
            return new Circle(A, B, C);
        }
    }
}