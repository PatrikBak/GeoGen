namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a tracer of information about inconsistencies between objects containers that happened. 
    /// For more information see the documentation of <see cref="InconsistentContainersException"/> and
    /// <see cref="UnresolvableInconsistencyException"/>.
    /// </summary>
    public interface IInconsistentContainersTracer
    {
        /// <summary>
        /// Traces the number of needed reconstructions of all the containers that were needed to resolve
        /// the inconsistency. This number shouldn't be zero.
        /// </summary>
        /// <param name="manager">The object manager that resolved the inconsistency.</param>
        /// <param name="attemptsToReconstruct">The number of reconstructions of all the containers that were needed to resolve the inconsistency.</param>
        void TraceResolvedInconsistency(IObjectsContainersManager manager, int attemptsToReconstruct);

        /// <summary>
        /// Traces that we've reached the maximal allowed number of attempts to reconstruct all the containers
        /// in order to solve an inconsistency.
        /// </summary>
        /// <param name="manager">The object manager that was resolving the inconsistency.</param>
        void TraceReachingMaximalNumberOfAttemptsToReconstructAllContainers(IObjectsContainersManager manager);

        /// <summary>
        /// Traces the number of unsuccessful attempts to reconstruct a container. This number shouldn't be zero.
        /// </summary>
        /// <param name="manager">The object manager that is parent of the reconstructed container.</param>
        /// <param name="container">The container that was reconstructed.</param>
        /// <param name="attemptsToReconstruct">The number of unsuccessful attempts to reconstruct the container.</param>
        void TraceUnsuccessfulAtemptsToReconstructOneContainer(IObjectsContainersManager manager, IObjectsContainer container, int attemptsToReconstruct);

        /// <summary>
        /// Traces that we've reached the maximal allowed number of attempts to reconstruct a container.
        /// </summary>
        /// <param name="manager">The object manager that is parent of the container that was being reconstructed.</param>
        /// <param name="container">The container that couldn't be reconstructed.</param>
        void TraceReachingMaximalNumberOfAttemptsToReconstructOneContainer(IObjectsContainersManager manager, IObjectsContainer container);
    }
}