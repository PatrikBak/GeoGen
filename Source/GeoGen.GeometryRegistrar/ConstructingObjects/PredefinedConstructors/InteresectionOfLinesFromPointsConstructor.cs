using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLinesFromPoints"/>>.
    /// </summary>
    public class IntersectionOfLinesFromPointsConstructor : PredefinedConstructorBase
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
            var point4 = (Point) input[3];

            // Create lines. 
            var line1 = new Line(point1, point2);
            var line2 = new Line(point3, point4);

            // If they are equal, the construction fails
            if (line1 == line2)
                return null;

            // Otherwise we can intersect them. 
            // If there is no intersection, the result will be null
            return line1.IntersectionWith(line2);
        }
    }
}