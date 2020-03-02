using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The implementation of <see cref="IConstructorFailureTracer"/> that does nothing.
    /// </summary>
    public class EmptyConstructorFailureTracer : IConstructorFailureTracer
    {
        /// <inheritdoc/>
        public void TraceUnexpectedConstructionFailure(ConstructedConfigurationObject configurationObject, IAnalyticObject[] analyticObjects, string message)
        {
        }
    }
}
