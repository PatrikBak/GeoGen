using System;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a tracker of inconsistencies between <see cref="IObjectsContainer"/>.
    /// (see the documentation of <see cref="InconsistentContainersException"/>). This 
    /// service has no affect on the algorithm whatsoever and is used only for 
    /// diagnostic purposes.
    /// </summary>
    public interface IInconsistenciesTracker
    {
        /// <summary>
        /// Occurs when a reconstruction of a <see cref="IObjectsContainer"/> fails. 
        /// </summary>
        void OnUnsuccessfulAttemptToReconstructContainer();

        /// <summary>
        /// Occurs when an inconsistency betweens containers is registered.
        /// </summary>
        void MarkInconsistency();
    }
}