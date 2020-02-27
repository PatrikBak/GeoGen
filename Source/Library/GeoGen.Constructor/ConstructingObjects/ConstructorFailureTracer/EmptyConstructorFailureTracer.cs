using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The implementation of <see cref="IConstructorFailureTracer"/> that does nothing.
    /// </summary>
    public class EmptyConstructorFailureTracer : IConstructorFailureTracer
    {
        /// <summary>
        /// Traces that unexpected behavior of analytic geometry construction has happened.
        /// </summary>
        /// <param name="configurationObject">The object that was being constructed.</param>
        /// <param name="analyticObjects">The input objects for the construction.</param>
        /// <param name="message">The message from the exception.</param>
        public void TraceUnexpectedConstructionFailure(ConstructedConfigurationObject configurationObject, IAnalyticObject[] analyticObjects, string message)
        {
        }
    }
}
