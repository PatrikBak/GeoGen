using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.ConsoleTest
{
    public class DefaultContexualContainerConstructionFailureTracer : IContexualContainerConstructionFailureTracer
    {
        public void TraceConstructionFailure(Configuration configuration, string message)
        {
        }

        public void TraceInconsistencyWhileConstructingContainer(Configuration configuration, ConfigurationObject problematicObject, string message)
        {
        }

        public void TraceReconstructionFailure(Configuration configuration, string message)
        {
        }

        public void TraceUnsuccessfulAttemptToReconstruct(Configuration configuration, string message)
        {
        }
    }
}