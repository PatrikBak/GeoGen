using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using System;
using System.IO;
using System.Linq;
using static GeoGen.Infrastructure.Log;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The implementation of <see cref="IConstructorFailureTracer"/> that writes
    /// failures to a file and logs them too. This file will be deleted at the initialization.
    /// </summary>
    public class ConstructorFailureTracer : IConstructorFailureTracer
    {
        #region Private fields

        /// <summary>
        /// The settings of the tracer.
        /// </summary>
        private readonly ConstructorFailureTracerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorFailureTracer"/> class.
        /// </summary>
        /// <param name="settings">The settings of the tracer.</param>
        public ConstructorFailureTracer(ConstructorFailureTracerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            // Delete the file first
            File.Delete(settings.FailureFilePath);
        }

        #endregion

        #region IConstructorFailureTracer implementation

        /// <summary>
        /// Traces that unexpected behavior of analytic geometry construction has happened.
        /// </summary>
        /// <param name="configurationObject">The object that was being constructed.</param>
        /// <param name="analyticObjects">The input objects for the construction.</param>
        /// <param name="message">The message from the exception.</param>
        public void TraceUnexpectedConstructionFailure(ConstructedConfigurationObject configurationObject, IAnalyticObject[] analyticObjects, string message)
        {
            // Prepare the initial information string
            var infoString = $"Exception while constructing {configurationObject.Construction.Name}({configurationObject.Construction.Signature}).";

            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                LoggingManager.LogWarning($"Construction: {infoString} See {_settings.FailureFilePath} for more detail.");

            // Add the input
            infoString += $"\n\nInput:\n\n{analyticObjects.Select(o => $"{o.GetType().Name}: {o}").ToJoinedString("\n").Indent(2)}";

            // Add the exception
            infoString += $"\n\nException message: {message}\n";

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FailureFilePath, append: true);

            // Write indented message to the file
            streamWriter.WriteLine($"- {infoString.Indent(2).TrimStart()}");
        }

        #endregion
    }
}
