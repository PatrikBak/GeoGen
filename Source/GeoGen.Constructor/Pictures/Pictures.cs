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

        /// <summary>
        /// The settings.
        /// </summary>
        protected readonly PicturesSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Pictures"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public Pictures(PicturesSettings settings)
            : this(settings, GeneralUtilities.ExecuteNTimes(settings.NumberOfPictures, () => new Picture()).ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pictures"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="pictures">The list of pictures that are contained in this collection.</param>
        protected Pictures(PicturesSettings settings, IReadOnlyList<Picture> pictures)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
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
            var pictures = new Pictures(_settings, picturesList);

            // Return them
            return pictures;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Performs a given function that might cause an <see cref="InconsistentPicturesException"/> and tries 
        /// to handle it by reconstructing all the pictures. If the exception couldn't be handled, throws an
        /// <see cref="UnresolvedInconsistencyException"/>.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        /// <param name="exceptionCallback">The action called after an <see cref="InconsistentPicturesException"/> occurs.</param>
        internal void ExecuteAndReconstructAtIncosistencies(Action action, Action<InconsistentPicturesException> exceptionCallback)
        {
            // Prepare a variable holding the number of attempts to reconstruct all the pictures
            var attempts = 0;

            // Repeat until we break
            while (true)
            {
                try
                {
                    // Try to call the action
                    action();

                    // If we got here, the action was successful
                    return;
                }
                // If there was an inconsistency...
                catch (InconsistentPicturesException e)
                {
                    // Call the callback
                    exceptionCallback(e);

                    // If we reached the maximal number of reconstructions, we can't do more
                    if (attempts == _settings.MaximalAttemptsToReconstructAllPictures)
                    {
                        // Make sure it's noted
                        throw new UnresolvedInconsistencyException(
                            "The reconstruction of the pictures failed, because the maximum number of attempts " +
                           $"{_settings.MaximalAttemptsToReconstructAllPictures} to do so has been reached.");
                    }

                    // Mark an attempt 
                    attempts++;

                    // Try to reconstruct all the pictures
                    foreach (var picture in _pictures)
                    {
                        // Reconstruct the given one
                        TryReconstruct(picture, out var success);

                        // If it failed, the whole reconstruction has
                        if (!success)
                        {
                            // Make sure it's noted
                            throw new UnresolvedInconsistencyException(
                                "The reconstruction of the pictures failed, because the maximum number of attempts " +
                               $"{_settings.MaximalAttemptsToReconstructOnePicture} to reconstructed a picture has been reached.");
                        }
                    }
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Tries to reconstruct a single picture for the maximal number of allowed times.
        /// </summary>
        /// <param name="picture">The picture to be reconstructed.</param>
        /// <param name="success">The parameter indicating if the reconstruction was successful.</param>
        private void TryReconstruct(Picture picture, out bool success)
        {
            // Prepare a variable holding the number of attempts to reconstruct
            var attempts = 0;

            // Try until we reach the maximal allowed number of attempts
            while (attempts < _settings.MaximalAttemptsToReconstructOnePicture)
            {
                // Mark an attempt
                attempts++;

                // Try to reconstruct
                picture.TryReconstruct(out var successful);

                // If it went fine, we can break
                if (successful)
                    break;
            }

            // Set whether we've succeeded based on the maximal number of reconstructions...
            success = attempts != _settings.MaximalAttemptsToReconstructOnePicture;
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<Picture> GetEnumerator() => _pictures.GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion        
    }
}