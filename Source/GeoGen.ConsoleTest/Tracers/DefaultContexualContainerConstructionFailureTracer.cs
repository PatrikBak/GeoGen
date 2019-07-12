using GeoGen.Constructor;
using GeoGen.Core;
using System;

namespace GeoGen.ConsoleTest
{
    public class DefaultContexualContainerConstructionFailureTracer : IContexualContainerConstructionFailureTracer
    {
        public void TraceConstructionFailure(Configuration configuration, string message)
        {
            Console.WriteLine($"[Warning] Contextual container construction failure: {message}");
        }

        public void TraceInconsistencyWhileConstructingContainer(Configuration configuration, ConfigurationObject problematicObject, string message)
        {
            Console.WriteLine($"[Error] Contextual container construction failed: {message}");
        }

        public void TraceReconstructionFailure(Configuration configuration, string message)
        {
            Console.WriteLine($"[Error] Contextual container reconstruction failure: {message}");
        }

        public void TraceUnsuccessfulAttemptToReconstruct(Configuration configuration, string message)
        {
            Console.WriteLine($"[Warning] Contextual container reconstruction failure: {message}");
        }
    }
}