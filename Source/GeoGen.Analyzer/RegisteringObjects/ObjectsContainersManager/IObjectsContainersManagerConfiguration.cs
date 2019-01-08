namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents the configuration for <see cref="ObjectsContainersManager"/>.
    /// </summary>
    public interface IObjectsContainersManagerConfiguration
    {
        /// <summary>
        /// The number of pictures to which a configuration is drawn and tested for theorems.
        /// </summary>
        int NumberOfContainers { get; }

        /// <summary>
        /// The maximal number of attempts to resolve inconsistencies between containers by reconstructing all of them.
        /// </summary>
        int MaximalAttemptsToReconstructAllContainers { get; }

        /// <summary>
        /// The maximal number of attempts to reconstruct a single objects container.
        /// </summary>
        int MaximalAttemptsToReconstructOneContainer { get; }
    }
}