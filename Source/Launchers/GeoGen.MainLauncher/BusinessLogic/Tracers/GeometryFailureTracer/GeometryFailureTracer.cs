using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.ProblemGenerator;
using GeoGen.Utilities;
using Serilog;
using System;
using System.IO;

namespace GeoGen.MainLauncher
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
            File.Delete(settings.FailureFilePath);
        }

        #endregion

        #region IGeometryFailureTracer implementation

        /// <inheritdoc/>
        public void InconstructiblePicturesByCloning(PicturesOfConfiguration previousPictures, Configuration newConfiguration, InconsistentPicturesException exception)
        {
            // Prepare the initial information string
            var infoString = $"Undrawable object into pictures.";

            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                Log.Warning("Object generation: {info} See {path} for more detail.", infoString, _settings.FailureFilePath);

            // Prepare the formatter for the configuration
            var formatter = new OutputFormatter(newConfiguration.AllObjects);

            // Add the data about how the object can be drawn
            infoString += $"\n\nThe object is the last object of the following defining configuration:\n\n{formatter.FormatConfiguration(newConfiguration).Indent(2)}";

            // Add the exception
            infoString += $"\n\nThe details of the exception: {exception.Format(formatter)}\n";

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FailureFilePath, append: true);

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
            // Prepare the initial information string
            var infoString = $"Undrawable object into a contextual picture.";

            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                Log.Warning("Object generation: {info} See {path} for more detail.", infoString, _settings.FailureFilePath);

            // Prepare the formatter for the configuration
            var formatter = new OutputFormatter(newConfigurationPictures.Configuration.AllObjects);

            // Add the data about how the object can be drawn
            infoString += $"\n\nThe object is the last object of the following defining configuration:\n\n{formatter.FormatConfiguration(newConfigurationPictures.Configuration).Indent(2)}";

            // Add the exception
            infoString += $"\n\nThe details of the exception: {exception.Format(formatter)}\n";

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FailureFilePath, append: true);

            // Write indented message to the file
            streamWriter.WriteLine($"- {infoString.Indent(3).TrimStart()}");
        }

        #endregion
    }
}
