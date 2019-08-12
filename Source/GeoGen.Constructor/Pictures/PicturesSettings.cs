namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents the configuration for <see cref="Pictures"/>.
    /// </summary>
    public class PicturesSettings
    {
        /// <summary>
        /// The number of pictures to which a configuration is drawn and tested for theorems.
        /// </summary>
        public int NumberOfPictures { get; set; }

        /// <summary>
        /// The maximal number of attempts to resolve inconsistencies between pictures by reconstructing all of them.
        /// </summary>
        public int MaximalAttemptsToReconstructAllPictures { get; set; }

        /// <summary>
        /// The maximal number of attempts to reconstruct a single picture.
        /// </summary>
        public int MaximalAttemptsToReconstructOnePicture { get; set; }
    }
}