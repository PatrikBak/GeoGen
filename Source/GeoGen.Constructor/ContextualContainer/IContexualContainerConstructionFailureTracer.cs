using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a service that traces failures to construct or reconstruct
    /// a <see cref="ContextualContainer"/>.
    /// </summary>
    public interface IContexualContainerConstructionFailureTracer
    {
        /// <summary>
        /// Traces that the container couldn't be constructed for a given configuration 
        /// which happened while adding a given problematic object.
        /// </summary>
        /// <param name="configuration">The configuration for which the container was being constructed.</param>
        /// <param name="problematicObject">The configuration object whose adding caused the problem.</param>
        /// <param name="message">The message containing more information about the construction.</param>
        void TraceInconsistencyWhileConstructingContainer(Configuration configuration, ConfigurationObject problematicObject, string message);

        /// <summary>
        /// Traces that the construction of the container representing a given configuration failed.
        /// </summary>
        /// <param name="configuration">The configuration for which the container was being constructed.</param>
        /// <param name="message">The message containing more information about the construction failure.</param>
        void TraceConstructionFailure(Configuration configuration, string message);

        /// <summary>
        /// Traces that the reconstruction of the container representing a given configuration was unsuccessful.
        /// </summary>
        /// <param name="configuration">The configuration for which the container was being reconstructed.</param>
        /// <param name="message">The message containing more information about the reconstruction.</param>
        void TraceUnsuccessfulAttemptToReconstruct(Configuration configuration, string message);

        /// <summary>
        /// Traces that the reconstruction of the container representing a given configuration failed.
        /// </summary>
        /// <param name="configuration">The configuration for which the container was being reconstructed.</param>
        /// <param name="message">The message containing more information about the failure.</param>
        void TraceReconstructionFailure(Configuration configuration, string message);
    }
}