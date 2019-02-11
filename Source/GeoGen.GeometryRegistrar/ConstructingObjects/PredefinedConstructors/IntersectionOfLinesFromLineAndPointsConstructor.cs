using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLinesFromLineAndPoints"/>>.
    /// </summary>
    public class IntersectionOfLinesFromLineAndPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override IAnalyticObject Construct(IAnalyticObject[] input)
        {
            // Get the line
            var line = (Line) input[0];

            // Get the points
            var point1 = (Point) input[1];
            var point2 = (Point) input[2];

            // Create the other line
            var otherLine = new Line(point1, point2);

            // If the lines are the same, then the construction is not possible
            if (line == otherLine)
                return null;

            // Otherwise we can intersect them. 
            // If there is no intersection, the result will be null
            return line.IntersectionWith(otherLine);
        }
    }
}