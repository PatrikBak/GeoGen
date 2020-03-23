using System;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// Represents the settings for <see cref="ProblemGenerator"/>.
    /// </summary>
    public class ProblemGeneratorSettings
    {
        #region Public properties

        /// <summary>
        /// The number of pictures to which a configuration is drawn.
        /// </summary>
        public int NumberOfPictures { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGeneratorSettings"/> class.
        /// </summary>
        /// <param name="numberOfPictures"><inheritdoc cref="NumberOfPictures" path="/summary"/></param>
        public ProblemGeneratorSettings(int numberOfPictures)
        {
            NumberOfPictures = numberOfPictures;

            // Ensure there are some pictures
            if (numberOfPictures <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfPictures), "The number of pictures must be at least 1");
        }

        #endregion
    }
}
