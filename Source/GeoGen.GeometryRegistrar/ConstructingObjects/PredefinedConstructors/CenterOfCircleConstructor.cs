using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// The <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.CenterOfCircle"/>>.
    /// </summary>
    public class CenterOfCircleConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Performs the actual construction of an analytic object based on the analytic objects given as an input.
        /// The order of the objects of the input is based on the <see cref="Arguments.FlattenedList"/>.
        /// </summary>
        /// <param name="input">The analytic objects to be used as an input.</param>
        /// <returns>The constructed analytic object, if the construction was successful; or null otherwise.</returns>
        protected override IAnalyticObject Construct(IAnalyticObject[] input) => ((Circle) input[0]).Center;
    }
}