using GeoGen.Constructor;
using GeoGen.Core;
using System;

namespace GeoGen.ConsoleTest
{
    public class DefaultGeometryConstructionFailureTracer : IGeometryConstructionFailureTracer
    {
        public void TraceInconsistencyWhileDrawingConfiguration(Configuration configuration, ConstructedConfigurationObject problematicObject, string message)
        {
            Console.WriteLine($"[Warning] Inconsistency while drawing configuration: {message}");
        }

        public void TraceInconsistencyWhileExaminingObject(Configuration configuration, ConstructedConfigurationObject problematicObject, string message)
        {
            Console.WriteLine($"[Warning] Inconsistency while examining object: {message}");
        }

        public void TraceUnresolvedInconsistencyWhileDrawingConfiguration(Configuration configuration, string message)
        {
            Console.WriteLine($"[Error] Unresolved inconsistency while drawing configuration: {message}");
        }

        public void TraceUnresolvedInconsistencyWhileExaminingObject(Configuration configuration, ConstructedConfigurationObject problematicObject, string message)
        {
            Console.WriteLine($"[Error] Unresolved inconsistency while examining object: {message}");
        }
    }
}