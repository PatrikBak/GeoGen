using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.CircumcircleFromPoints"/>>.
    /// </summary>
    public class CircumcircleFromPointsConstructor : PredefinedConstructorBase
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
            var point1 = (Point) input[0];
            var point2 = (Point) input[1];
            var point3 = (Point) input[2];

            // If the points are collinear, the construction can't be done
            if (AnalyticHelpers.AreCollinear(point1, point2, point3))
                return null;

            // Otherwise construct their circumcircle
            return new Circle(point1, point2, point3);
        }
    }
}