using GeoGen.Core;
using System.Collections.Generic;

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
        /// <param name="objects">The objects that were being constructed.</param>
        /// <param name="problematicObject">The configuration object whose adding caused the problem.</param>
        /// <param name="message">The message containing more information about the construction.</param>
        void TraceInconsistencyWhileConstructingPicture(IReadOnlyList<ConfigurationObject> objects, ConfigurationObject problematicObject, string message);

        /// <summary>
        /// Traces that the construction of the picture representing a given configuration failed.
        /// </summary>
        /// <param name="objects">The objects that were being constructed.</param>
        /// <param name="message">The message containing more information about the construction failure.</param>
        void TraceConstructionFailure(IReadOnlyList<ConfigurationObject> objects, string message);
    }
}