using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.PerpendicularLineFromPoints"/>>.
    /// </summary>
    public class PerpendicularLineFromPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override AnalyticObject Construct(AnalyticObject[] input)
        {
            // Pull the point from which we erect the perpendicular line
            var sourcePoint = (Point) input[0];

            // Pull the line points
            var linePoint1 = (Point) input[1];
            var linePoint2 = (Point) input[2];

            // Construct the result
            return new Line(linePoint1, linePoint2).PerpendicularLine(sourcePoint);
        }
    }
}