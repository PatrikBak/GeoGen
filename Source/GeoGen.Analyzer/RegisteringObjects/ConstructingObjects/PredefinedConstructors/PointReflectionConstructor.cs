using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.PointReflection"/>>.
    /// </summary>
    public class PointReflectionConstructor : PredefinedConstructorBase
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

            // We want (X + point1) / 2 = point2, from which we easily have the result
            return 2 * point2 - point1;
        }
    }
}