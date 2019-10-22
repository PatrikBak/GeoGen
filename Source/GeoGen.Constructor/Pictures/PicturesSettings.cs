namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents the settings for <see cref="Pictures"/>.
    /// </summary>
    public class PicturesSettings
    {
        #region Public properties

        /// <summary>
        /// The number of pictures to which a configuration is drawn.
        /// </summary>
        public int NumberOfPictures { get; }

        /// <summary>
        /// The maximal number of attempts to resolve inconsistencies between pictures by reconstructing all of them.
        /// </summary>
        public int MaximalAttemptsToReconstructAllPictures { get; }

        /// <summary>
        /// The maximal number of attempts to reconstruct a single picture.
        /// </summary>
        public int MaximalAttemptsToReconstructOnePicture { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesSettings"/> class.
        /// </summary>
        /// <param name="numberOfPictures">The number of pictures to which a configuration is drawn.</param>
        /// <param name="maximalAttemptsToReconstructAllPictures">The maximal number of attempts to resolve inconsistencies between pictures by reconstructing all of them.</param>
        /// <param name="maximalAttemptsToReconstructOnePicture">The maximal number of attempts to reconstruct a single picture.</param>
        public PicturesSettings(int numberOfPictures, int maximalAttemptsToReconstructAllPictures, int maximalAttemptsToReconstructOnePicture)
        {
            NumberOfPictures = numberOfPictures;
            MaximalAttemptsToReconstructAllPictures = maximalAttemptsToReconstructAllPictures;
            MaximalAttemptsToReconstructOnePicture = maximalAttemptsToReconstructOnePicture;
        }

        #endregion
    }
}