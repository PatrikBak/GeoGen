using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLines"/>>.
    /// </summary>
    public class IntersectionOfLinesConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override AnalyticObject Construct(AnalyticObject[] input)
        {
            // Get the lines
            var line1 = (Line) input[0];
            var line2 = (Line) input[1];

            // Intersection them. If there is no intersection, the result will be null
            return line1.IntersectionWith(line2);
        }
    }
}