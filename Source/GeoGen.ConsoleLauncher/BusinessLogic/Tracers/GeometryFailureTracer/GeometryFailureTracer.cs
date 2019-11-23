using GeoGen.Algorithm;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.IO;
using System.Linq;
using static GeoGen.ConsoleLauncher.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The implementation of <see cref="IGeometryFailureTracer"/> that writes
    /// failures to a file and logs them too. This file will be deleted at the initialization.
    /// </summary>
    public class GeometryFailureTracer : IGeometryFailureTracer
    {
        #region Private fields

        /// <summary>
        /// The settings of the tracer.
        /// </summary>
        private readonly GeometryFailureTracerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryFailureTracer"/> class.
        /// </summary>
        /// <param name="settings">The settings of the tracer.</param>
        public GeometryFailureTracer(GeometryFailureTracerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            // Delete the file first
            File.Delete(settings.FailuresFilePath);
        }

        #endregion

        #region IGeometryFailureTracer implementation

        /// <summary>
        /// Traces that there is an object that cannot be consistently drawn to the pictures holding all the objects
        /// that the algorithm has generated.
        /// </summary>
        /// <param name="constructedObject">The object that couldn't be drawn consistently.</param>
        /// <param name="initialLooseObjects">The loose objects of the initial configuration without which we can't provide full information about the constructed object.</param>
        /// <param name="pictures">The pictures to which the object was attempted to be drawn.</param>
        /// <param name="exception">The inner inconsistency exception that caused the issue.</param>
        public void UndrawableObjectInBigPicture(ConstructedConfigurationObject constructedObject, LooseObjectsHolder initialLooseObjects, Pictures pictures, InconsistentPicturesException exception)
        {
            // Prepare the configuration that can be used to fully define the constructed object
            // and the objects mentioned in the exception
            var configuration = new Configuration(initialLooseObjects,
                // Take the inner exception objects plus the undrawable object
                exception.GetInnerObjects().Concat(constructedObject)
                // Use the helper method to get all the constructed object needed to construct these objects in the right order
                .GetDefiningObjects().OfType<ConstructedConfigurationObject>().ToArray());

            // Prepare the formatter for the configuration
            var formatter = new OutputFormatter(configuration.AllObjects);

            // Prepare the initial information string
            var infoString = $"Undrawable object into the big picture (with {pictures.First().Count()} objects).";

            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                LoggingManager.LogWarning($"Object generation: {infoString} See {_settings.FailuresFilePath} for more detail.");

            // Add the data about how the object can be drawn
            infoString += $"\n\nConsider the following configuration defining all needed objects:\n\n{formatter.FormatConfiguration(configuration).Indent(2)}";

            // Add the exception
            infoString += $"\n\nObject '{formatter.GetObjectName(constructedObject)}' couldn't be drawn. The details of the exception: {exception.Format(formatter)}\n";

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FailuresFilePath, append: true);

            // Write indented message to the file
            streamWriter.WriteLine($"- {infoString.Indent(3).TrimStart()}");
        }

        /// <summary>
        /// Traces that a given contextual picture couldn't be cloned and extended with the new object
        /// already drawn in pictures representing some configuration.
        /// </summary>
        /// <param name="previousContextualPicture">The contextual picture that was correct and failed to add the new object.</param>
        /// <param name="newConfigurationPictures">The pictures holding geometry data of the new object that was added.</param>
        /// <param name="exception">The inner inconsistency exception that caused the issue.</param>
        public void InconstructibleContextualPictureByCloning(ContextualPicture previousContextualPicture, PicturesOfConfiguration newConfigurationPictures, InconsistentPicturesException exception)
        {
            // Prepare the formatter for the configuration
            var formatter = new OutputFormatter(newConfigurationPictures.Configuration.AllObjects);

            // Prepare the initial information string
            var infoString = $"Undrawable object into a contextual picture.";

            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                LoggingManager.LogWarning($"Object generation: {infoString} See {_settings.FailuresFilePath} for more detail.");

            // Add the data about how the object can be drawn
            infoString += $"\n\nThe object is the last object of the following defining configuration:\n\n{formatter.FormatConfiguration(newConfigurationPictures.Configuration).Indent(2)}";

            // Add the exception
            infoString += $"\n\nThe details of the exception: {exception.Format(formatter)}\n";

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FailuresFilePath, append: true);

            // Write indented message to the file
            streamWriter.WriteLine($"- {infoString.Indent(3).TrimStart()}");
        }

        #endregion
    }
}
