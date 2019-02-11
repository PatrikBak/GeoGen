namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// Represents a tracer of information about unsuccessful reconstructions of a <see cref="IContextualContainer"/>.
    /// </summary>
    public interface IUnsuccessfulReconstructionsTracer
    {
        /// <summary>
        /// Traces that we've had some unsuccessful attempts to reconstruct the contextual container.
        /// </summary>
        /// <param name="container">The contextual container that was reconstructed.</param>
        /// <param name="manager">The manager that holds all the containers of objects.</param>
        /// <param name="numberOfUnsuccessfulAttempts">The number of unsuccessful attempts to reconstruct the container.</param>
        void TraceUnsucessfullAttemptsToReconstruct(IContextualContainer container, IObjectsContainersManager manager, int numberOfUnsuccessfulAttempts);

        /// <summary>
        /// Traces that we've reached the maximal number of attempts to reconstruct a contextual container.
        /// </summary>
        /// <param name="container">The contextual container that was being reconstructed.</param>
        /// <param name="manager">The manager that holds all the containers of objects.</param>
        void TraceReachingMaximalNumberOfAttemptsToReconstruct(IContextualContainer container, IObjectsContainersManager manager);
    }
}