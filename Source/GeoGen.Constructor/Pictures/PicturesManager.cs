using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IPicturesManager"/>. 
    /// </summary>
    public class PicturesManager : IPicturesManager
    {
        #region Private fields

        /// <summary>
        /// The list of all the pictures managed by this manager.
        /// </summary>
        private readonly List<IPicture> _pictures;

        /// <summary>
        /// The settings for the manager.
        /// </summary>
        private readonly PicturesManagerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PicturesManager"/> class.
        /// </summary>
        /// <param name="settings">The settings for the manager.</param>
        /// <param name="factory">The factory for creating empty pictures.</param>
        public PicturesManager(PicturesManagerSettings settings, IPictureFactory factory)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            #region Make sure the settings have correct values

            if (_settings.NumberOfPictures <= 0)
                throw new ArgumentOutOfRangeException(nameof(PicturesManagerSettings.NumberOfPictures), settings.NumberOfPictures, "There has to be at least one picture.");

            if (_settings.MaximalAttemptsToReconstructOnePicture < 0)
                throw new ArgumentOutOfRangeException(nameof(PicturesManagerSettings.MaximalAttemptsToReconstructOnePicture), settings.MaximalAttemptsToReconstructOnePicture, "The value cannot be negative.");

            if (_settings.MaximalAttemptsToReconstructAllPictures < 0)
                throw new ArgumentOutOfRangeException(nameof(PicturesManagerSettings.MaximalAttemptsToReconstructAllPictures), settings.MaximalAttemptsToReconstructAllPictures, "The value cannot be negative.");

            #endregion

            // Create the requested number of pictures
            _pictures = Enumerable.Range(0, settings.NumberOfPictures).Select(i => factory.CreatePicture()).ToList();
        }

        #endregion

        #region IObjectsPicturesManager implementation

        /// <summary>
        /// Performs a given function that might cause an <see cref="InconsistentPicturesException"/> and tries 
        /// to handle it by reconstructing all the pictures. If the exception couldn't be handled, throws an
        /// <see cref="UnresolvedInconsistencyException"/>.
        /// </summary>
        /// <param name="function">The action to be executed.</param>
        /// <param name="exceptionCallback">The action called after an <see cref="InconsistentPicturesException"/> occurs.</param>
        public void ExecuteAndResolvePossibleIncosistencies(Action action, Action<InconsistentPicturesException> exceptionCallback)
        {
            // Prepare a variable holding the number of attempts to reconstruct all the pictures
            var attempts = 0;

            // Try to perform the function until we reach the maximal number of attempts
            while (attempts < _settings.MaximalAttemptsToReconstructAllPictures)
            {
                try
                {
                    // Call the action
                    action();

                    // If we got here, we're happy and we can break the cycle
                    break;
                }
                // If there was an inconsistency...
                catch (InconsistentPicturesException e)
                {
                    // Call the callback
                    exceptionCallback(e);

                    // Mark an attempt 
                    attempts++;

                    // Try to reconstruct all the pictures
                    foreach (var picture in _pictures)
                    {
                        // Reconstruct the given one
                        TryReconstruct(picture, out var success);

                        // If it failed, the whole reconstruction has
                        if (!success)
                            throw new UnresolvedInconsistencyException(
                                "The reconstruction of the pictures failed, because the maximum number of attempts " +
                               $"{_settings.MaximalAttemptsToReconstructOnePicture} to reconstructed a picture has been reached.");
                    }
                }
            }

            // If we reached the maximal number of reconstructions, we've failed as well
            if (attempts == _settings.MaximalAttemptsToReconstructAllPictures)
                throw new UnresolvedInconsistencyException(
                    "The reconstruction of the pictures failed, because the maximum number of attempts " +
                   $"{_settings.MaximalAttemptsToReconstructAllPictures} to do so has been reached.");
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Tries to reconstruct a single picture for the maximal number of allowed times.
        /// </summary>
        /// <param name="picture">The picture to be reconstructed.</param>
        /// <param name="success">The parameter indicating if the reconstruction was successful.</param>
        private void TryReconstruct(IPicture picture, out bool success)
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
        public IEnumerator<IPicture> GetEnumerator() => _pictures.GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion        
    }
}