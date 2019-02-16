namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents the configuration for <see cref="ObjectsContainersManager"/>.
    /// </summary>
    public class ObjectsContainersManagerSettings
    {
        /// <summary>
        /// The number of pictures to which a configuration is drawn and tested for theorems.
        /// </summary>
        public int NumberOfContainers { get; set; }

        /// <summary>
        /// The maximal number of attempts to resolve inconsistencies between containers by reconstructing all of them.
        /// </summary>
        public int MaximalAttemptsToReconstructAllContainers { get; set; }

        /// <summary>
        /// The maximal number of attempts to reconstruct a single objects container.
        /// </summary>
        public int MaximalAttemptsToReconstructOneContainer { get; set; }
    }
}