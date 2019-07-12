using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a service that traces failures to construct or reconstruct
    /// a <see cref="ContextualPicture"/>.
    /// </summary>
    public interface IContexualPictureConstructionFailureTracer
    {
        /// <summary>
        /// Traces that the picture couldn't be constructed for a given configuration 
        /// which happened while adding a given problematic object.
        /// </summary>
        /// <param name="configuration">The configuration for which the picture was being constructed.</param>
        /// <param name="problematicObject">The configuration object whose adding caused the problem.</param>
        /// <param name="message">The message containing more information about the construction.</param>
        void TraceInconsistencyWhileConstructingPicture(Configuration configuration, ConfigurationObject problematicObject, string message);

        /// <summary>
        /// Traces that the construction of the picture representing a given configuration failed.
        /// </summary>
        /// <param name="configuration">The configuration for which the picture was being constructed.</param>
        /// <param name="message">The message containing more information about the construction failure.</param>
        void TraceConstructionFailure(Configuration configuration, string message);

        /// <summary>
        /// Traces that the reconstruction of the picture representing a given configuration was unsuccessful.
        /// </summary>
        /// <param name="configuration">The configuration for which the picture was being reconstructed.</param>
        /// <param name="message">The message containing more information about the reconstruction.</param>
        void TraceUnsuccessfulAttemptToReconstruct(Configuration configuration, string message);

        /// <summary>
        /// Traces that the reconstruction of the picture representing a given configuration failed.
        /// </summary>
        /// <param name="configuration">The configuration for which the picture was being reconstructed.</param>
        /// <param name="message">The message containing more information about the failure.</param>
        void TraceReconstructionFailure(Configuration configuration, string message);
    }
}