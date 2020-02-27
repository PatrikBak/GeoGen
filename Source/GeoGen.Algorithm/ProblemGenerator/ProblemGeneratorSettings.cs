using GeoGen.Core;
using System;

namespace GeoGen.Algorithm
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

        /// <summary>
        /// Indicates if we should automatically exclude configurations that are not symmetric according to <see cref="Configuration.IsSymmetric"/>.
        /// </summary>
        public bool ExcludeAsymmetricConfigurations { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGeneratorSettings"/> class.
        /// </summary>
        /// <param name="numberOfPictures">The number of pictures to which a configuration is drawn.</param>
        /// <param name="excludeAsymmetricConfigurations">Indicates if we should automatically exclude configurations that are not symmetric according to <see cref="Configuration.IsSymmetric"/>.</param>
        public ProblemGeneratorSettings(int numberOfPictures, bool excludeAsymmetricConfigurations)
        {
            NumberOfPictures = numberOfPictures;
            ExcludeAsymmetricConfigurations = excludeAsymmetricConfigurations;

            // Ensure there are some pictures
            if (numberOfPictures <= 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfPictures), "The number of pictures must be at least 1");
        }

        #endregion
    }
}
