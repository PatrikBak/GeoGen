namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents the configuration for <see cref="ContextualContainer"/>.
    /// </summary>
    public interface IContextualContainerConfiguration
    {
        /// <summary>
        /// The maximal allowed number of attempts to reconstruct the contextual container.
        /// </summary>
        int MaximalNumberOfAttemptsToReconstruct { get; }
    }
}
