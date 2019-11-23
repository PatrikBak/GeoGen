using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremProver;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using static GeoGen.ConsoleLauncher.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The implementation of <see cref="ISubtheoremDeriverGeometryFailureTracer"/> that writes
    /// failures to a file and logs them too. This file will be deleted at the initialization.
    /// </summary>
    public class SubtheoremDeriverGeometryFailureTracer : ISubtheoremDeriverGeometryFailureTracer
    {
        #region Private fields

        /// <summary>
        /// The settings of the tracer.
        /// </summary>
        private readonly SubtheoremDeriverGeometryFailureTracerSettings _settings;

        /// <summary>
        /// The set of already logged objects with respect to the configurations.
        /// </summary>
        private readonly HashSet<(string configurationString, string objectString)> _alreadyLoggedObjects;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtheoremDeriverGeometryFailureTracer"/> class.
        /// </summary>
        /// <param name="settings">The settings of the tracer.</param>
        public SubtheoremDeriverGeometryFailureTracer(SubtheoremDeriverGeometryFailureTracerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            // Create the set for already logged objects
            _alreadyLoggedObjects = new HashSet<(string configurationString, string objectString)>();

            // Delete the file first
            File.Delete(settings.FailuresFilePath);
        }

        #endregion

        #region ISubtheoremDeriverGeometryFailureTracer implementation

        /// <summary>
        /// Traces that drawing of an object using the input from given pictures couldn't be performed consistently.
        /// </summary>
        /// <param name="constructedObject">The object that couldn't be drawn consistently.</param>
        /// <param name="pictures">The pictures from which the input for the construction was taken.</param>
        /// <param name="exception">The inner inconsistency exception that caused the issue.</param>
        public void UndrawableObject(ConstructedConfigurationObject constructedObject, PicturesOfConfiguration pictures, InconsistentPicturesException exception)
        {
            // Prepare the formatter for the configuration
            var formatter = new OutputFormatter(pictures.Configuration.AllObjects.Concat(constructedObject));

            // Format the configuration
            var configurationString = formatter.FormatConfiguration(pictures.Configuration).Indent(2);

            // Format the object
            var objectString = formatter.FormatConfigurationObject(constructedObject).Indent(2);

            // If the object is already logged with respect to the configuration, don't do it again
            if (_alreadyLoggedObjects.Contains((configurationString, objectString)))
                return;

            // Otherwise add that the object has been logged
            _alreadyLoggedObjects.Add((configurationString, objectString));

            // Prepare the initial information string
            var infoString = $"Undrawable object.";

            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                LoggingManager.LogWarning($"Subtheorem examination: {infoString} See {_settings.FailuresFilePath} for more detail.");

            // Add the data about the correct configuration
            infoString += $"\n\nThis is the correctly drawn configuration:\n\n{configurationString}";

            // Add the data about the object
            infoString += $"\n\nThe object '{formatter.GetObjectName(constructedObject)} = {objectString.Trim()}' couldn't be drawn to it.";

            // Add the exception
            infoString += $"\n\nThe details of the exception: {exception.Format(formatter)}\n";

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FailuresFilePath, append: true);

            // Write indented message to the file
            streamWriter.WriteLine($"- {infoString.Indent(3).TrimStart()}");
        }

        /// <summary>
        /// Traces that an examination via <see cref="ContextualPicture.GetGeometricObject(IReadOnlyDictionary{Picture, AnalyticGeometry.IAnalyticObject})"/>
        /// couldn't be performed due to an inconsistency.
        /// </summary>
        /// <param name="constructedObject">The object that couldn't be examined consistently.</param>
        /// <param name="contextualPicture">The contextual picture where that was used to perform the examination.</param>
        /// <param name="exception">The inner inconsistency exception that caused the issue.</param>
        public void UnexaminableObjectInContextualPicture(ConstructedConfigurationObject constructedObject, ContextualPicture contextualPicture, InconsistentPicturesException exception)
        {
            // Prepare the formatter for the configuration
            var formatter = new OutputFormatter(contextualPicture.Pictures.Configuration.AllObjects.Concat(constructedObject));

            // Format the configuration
            var configurationString = formatter.FormatConfiguration(contextualPicture.Pictures.Configuration).Indent(2);

            // Format the object
            var objectString = formatter.FormatConfigurationObject(constructedObject).Indent(2);

            // If the object is already logged with respect to the configuration, don't do it again
            if (_alreadyLoggedObjects.Contains((configurationString, objectString)))
                return;

            // Otherwise add that the object has been logged
            _alreadyLoggedObjects.Add((configurationString, objectString));

            // Prepare the initial information string
            var infoString = $"Unexaminable object in a contextual picture.";

            // Log it with the reference to more detail in the file
            LoggingManager.LogWarning($"Subtheorem examination: {infoString} See {_settings.FailuresFilePath} for more detail.");

            // Add the data about the correct configuration
            infoString += $"\n\nThis is the correctly drawn configuration:\n\n{configurationString}";

            // Add the data about the object
            infoString += $"\n\nThe object '{formatter.GetObjectName(constructedObject)} = {objectString.Trim()}' couldn't be examined correctly.";

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