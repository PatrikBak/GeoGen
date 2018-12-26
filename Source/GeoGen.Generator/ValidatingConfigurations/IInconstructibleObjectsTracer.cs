using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a tracer of inconstructible <see cref="ConfigurationObject"/>, for example
    /// the intersection of parallel lines (which can be defined formally).
    /// </summary>
    public interface IInconstructibleObjectsTracer
    {
        /// <summary>
        /// Trace that we found an inconstructible object.
        /// </summary>
        /// <param name="configurationObject">The inconstructible object.</param>
        void TraceInconstructibleObject(ConfigurationObject configurationObject);
    }
}
