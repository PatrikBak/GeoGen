using GeoGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a collection of <see cref="Picture"/>s. 
    /// </summary>
    public class Pictures : IEnumerable<Picture>
    {
        #region Private fields

        /// <summary>
        /// The list of pictures that are contained in this collection.
        /// </summary>
        protected readonly IReadOnlyList<Picture> _pictures;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Pictures"/> class.
        /// </summary>
        /// <param name="numberOfPictures">The number of pictures these pictures hold.</param>
        public Pictures(int numberOfPictures)
            : this(GeneralUtilities.ExecuteNTimes(numberOfPictures, () => new Picture()).ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pictures"/> class.
        /// </summary>
        /// <param name="pictures">The list of pictures that these pictures hold.</param>
        public Pictures(IReadOnlyList<Picture> pictures)
        {
            _pictures = pictures ?? throw new ArgumentNullException(nameof(pictures));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Clones the pictures.
        /// </summary>
        /// <returns>The cloned pictures.</returns>
        public Pictures Clone()
        {
            // Clone the pictures
            var picturesList = _pictures.Select(picture => picture.Clone()).ToList();

            // Create a pictures instance with the cloned pictures
            var pictures = new Pictures(picturesList);

            // Return them
            return pictures;
        }

        #endregion

        #region IEnumerable implementation

        /// <inheritdoc/>
        public IEnumerator<Picture> GetEnumerator() => _pictures.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion        
    }
}