using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a tracer of <see cref="ConfigurationObject"/> that are not formally equal, 
    /// but are equal geometrically (after drawing). For example: The midpoint M of AB is formally
    /// equal to the midpoint of BA, but not the midpoint of CD, where C is the midpoint of AM
    /// and D is the midpoint of BM.
    /// </summary>
    public interface IEqualObjectsTracer
    {
        /// <summary>
        /// Traces that we found two geometrically equal objects.
        /// </summary>
        /// <param name="olderObject">The object that was constructed first.</param>
        /// <param name="newerObject">The object that is geometrically equal to the first one that was constructed later.</param>
        void TraceEqualObjects(ConfigurationObject olderObject, ConfigurationObject newerObject);
    }
}
