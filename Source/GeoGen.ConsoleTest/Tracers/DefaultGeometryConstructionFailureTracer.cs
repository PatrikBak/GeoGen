using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.ConsoleTest
{
    public class DefaultGeometryConstructionFailureTracer : IGeometryConstructionFailureTracer
    {
        public void TraceInconsistencyWhileDrawingConfiguration(Configuration configuration, ConstructedConfigurationObject problematicObject, string message)
        {
        }

        public void TraceInconsistencyWhileExaminingObject(Configuration configuration, ConstructedConfigurationObject problematicObject, string message)
        {
        }

        public void TraceUnresolvedInconsistencyWhileDrawingConfiguration(Configuration configuration, string message)
        {
        }

        public void TraceUnresolvedInconsistencyWhileExaminingObject(Configuration configuration, ConstructedConfigurationObject problematicObject, string message)
        {
        }
    }
}