using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using Serilog;

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

        /// <inheritdoc/>
        public void TraceUnexpectedConstructionFailure(ConstructedConfigurationObject configurationObject, IAnalyticObject[] analyticObjects, string message)
        {
            // Prepare the initial information string
            var infoString = $"Exception while constructing {configurationObject.Construction.Name}({configurationObject.Construction.Signature}).";

            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                Log.Warning("Construction: {info} See {path} for more detail.", infoString, _settings.FailureFilePath);

            // Add the input
            infoString += $"\n\nInput:\n\n{analyticObjects.Select(analyticObject => $"{analyticObject.GetType().Name}: {analyticObject}").ToJoinedString("\n").Indent(2)}";

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
