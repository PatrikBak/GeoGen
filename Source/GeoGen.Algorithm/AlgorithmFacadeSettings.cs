using System;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents the settings for <see cref="AlgorithmFacade"/>.
    /// </summary>
    public class AlgorithmFacadeSettings
    {
        #region Public properties

        /// <summary>
        /// The number of pictures to which a configuration is drawn.
        /// </summary>
        public int NumberOfPictures { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmFacadeSettings"/> class.
        /// </summary>
        /// <param name="numberOfPictures">The number of pictures to which a configuration is drawn.</param>
        public AlgorithmFacadeSettings(int numberOfPictures)
        {
            // Check validity
            if (numberOfPictures <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfPictures), "The number of pictures must be at least 1");

            // Set the value
            NumberOfPictures = numberOfPictures;
        }

        #endregion
    }
}
