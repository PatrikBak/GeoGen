using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents <see cref="Pictures"/> holding also a <see cref="Core.Configuration"/> that is drawn in the pictures.
    /// </summary>
    public class PicturesOfConfiguration : Pictures
    {
        #region Public properties

        /// <summary>
        /// The configuration that is drawn in the pictures.
        /// </summary>
        public Configuration Configuration { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesOfConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that is drawn in the pictures.</param>
        /// <param name="numberOfPictures">The number of pictures these pictures hold.</param>
        public PicturesOfConfiguration(Configuration configuration, int numberOfPictures)
            : this(configuration, GeneralUtilities.ExecuteNTimes(numberOfPictures, () => new Picture()).ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesOfConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that is drawn in the pictures.</param>
        /// <param name="pictures">The list of pictures that these pictures hold.</param>
        private PicturesOfConfiguration(Configuration configuration, IReadOnlyList<Picture> pictures)
            : base(pictures)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Clones the pictures.
        /// </summary>
        /// <returns>The cloned pictures.</returns>
        public new PicturesOfConfiguration Clone()
        {
            // Clone the pictures
            var picturesList = _pictures.Select(picture => picture.Clone()).ToList();

            // Create a pictures instance with the cloned pictures
            var pictures = new PicturesOfConfiguration(Configuration, picturesList);

            // Return them
            return pictures;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Clones the pictures.
        /// </summary>
        /// <param name="newConfiguration">The new configuration of the pictures.</param>
        /// <returns>The cloned pictures.</returns>
        internal PicturesOfConfiguration Clone(Configuration newConfiguration)
        {
            // Clone the pictures
            var picturesList = _pictures.Select(picture => picture.Clone()).ToList();

            // Create a pictures instance with the cloned pictures
            var pictures = new PicturesOfConfiguration(newConfiguration, picturesList);

            // Return them
            return pictures;
        }

        #endregion
    }
}