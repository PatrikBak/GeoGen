using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.InternalAngleBisectorFromPoints"/>>.
    /// </summary>
    public class InternalAngleBisectorFromPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override IAnalyticObject Construct(IAnalyticObject[] input)
        {
            // Get the rays intersection
            var intersection = (Point) input[0];

            // Get the points on the rays
            var point1 = (Point) input[1];
            var point2 = (Point) input[2];

            // If they are collinear, don't perform the construction
            // (it would be possible, but it would be unnecessarily 
            // equivalent to the perpendicular line construction)
            if (AnalyticHelpers.AreCollinear(point1, point2, intersection))
                return null;

            // Otherwise construct the result
            return AnalyticHelpers.InternalAngleBisector(intersection, point1, point2);
        }
    }
}