using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a service that traces failures of <see cref="IGeometryConstructor"/> to 
    /// draw objects consistently across more pictures.
    /// </summary>
    public interface IGeometryConstructionFailureTracer
    {
        /// <summary>
        /// Traces that a given configuration couldn't be drawn consistently which happened
        /// while constructing a given problematic object.
        /// </summary>
        /// <param name="configuration">The configuration being drawn.</param>
        /// <param name="problematicObject">The configuration object whose drawing caused the problem.</param>
        /// <param name="message">The message containing more information while this inconsistency happened.</param>
        void TraceInconsistencyWhileDrawingConfiguration(Configuration configuration, ConstructedConfigurationObject problematicObject, string message);

        /// <summary>
        /// Traces that drawing a given configuration failed.
        /// </summary>
        /// <param name="configuration">The configuration being drawn.</param>
        /// <param name="message">The message containing more information about the unresolved inconsistencies.</param>
        void TraceUnresolvedInconsistencyWhileDrawingConfiguration(Configuration configuration, string message);

        /// <summary>
        /// Traces that a given configuration couldn't be drawn consistently together with a given object.
        /// </summary>
        /// <param name="configuration">The configuration that is drawn.</param>
        /// <param name="problematicObject">The configuration object whose examining caused the problem.</param>
        /// <param name="message">The message containing more information while this inconsistency happened.</param>
        void TraceInconsistencyWhileExaminingObject(Configuration configuration, ConstructedConfigurationObject problematicObject, string message);

        /// <summary>
        /// Traces that drawing a given configuration together with a given object failed.
        /// </summary>
        /// <param name="configuration">The configuration that is drawn.</param>
        /// <param name="problematicObject"></param>
        /// <param name="message">The message containing more information about the unresolved inconsistencies.</param>
        void TraceUnresolvedInconsistencyWhileExaminingObject(Configuration configuration, ConstructedConfigurationObject problematicObject, string message);
    }
}
