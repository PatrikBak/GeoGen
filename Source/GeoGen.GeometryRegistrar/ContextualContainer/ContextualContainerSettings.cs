namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// Represents the configuration for <see cref="ContextualContainer"/>.
    /// </summary>
    public class ContextualContainerSettings
    {
        /// <summary>
        /// The maximal allowed number of attempts to reconstruct the contextual container.
        /// </summary>
        public int MaximalNumberOfAttemptsToReconstruct { get; set; }
    }
}