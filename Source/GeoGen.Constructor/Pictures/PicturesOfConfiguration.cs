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
        /// Gets the configuration that is drawn in the pictures.
        /// </summary>
        public Configuration Configuration { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesOfConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that is drawn in the pictures.</param>
        /// <param name="settings">The settings.</param>
        public PicturesOfConfiguration(Configuration configuration, PicturesSettings settings)
            : this(configuration, settings, GeneralUtilities.ExecuteNTimes(settings.NumberOfPictures, () => new Picture()).ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesOfConfiguration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that is drawn in the pictures.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="pictures">The list of pictures that are contained in this collection.</param>
        private PicturesOfConfiguration(Configuration configuration, PicturesSettings settings, IReadOnlyList<Picture> pictures)
            : base(settings, pictures)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Clones all the pictures and returns them as a <see cref="Pictures"/>, i.e. pictures without configuration.
        /// </summary>
        /// <returns>The clones pictures.</returns>
        public Pictures CloneAsRegularPictures() => Clone();

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
            var pictures = new PicturesOfConfiguration(newConfiguration, _settings, picturesList);

            // Return them
            return pictures;
        }

        #endregion
    }
}