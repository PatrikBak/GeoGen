using GeoGen.Constructor;
using GeoGen.Core;
using System;

namespace GeoGen.ConsoleTest
{
    public class DefaultContexualPictureConstructionFailureTracer : IContexualPictureConstructionFailureTracer
    {
        public void TraceConstructionFailure(Configuration configuration, string message)
        {
            Console.WriteLine($"[Warning] Contextual picture construction failure: {message}");
        }

        public void TraceInconsistencyWhileConstructingPicture(Configuration configuration, ConfigurationObject problematicObject, string message)
        {
            Console.WriteLine($"[Error] Contextual picture construction failed: {message}");
        }

        public void TraceReconstructionFailure(Configuration configuration, string message)
        {
            Console.WriteLine($"[Error] Contextual picture reconstruction failure: {message}");
        }

        public void TraceUnsuccessfulAttemptToReconstruct(Configuration configuration, string message)
        {
            Console.WriteLine($"[Warning] Contextual picture reconstruction failure: {message}");
        }
    }
}