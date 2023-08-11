using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.TheoremRanker;
using GeoGen.TheoremSorter;
using Serilog;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The implementation of <see cref="ISortingGeometryFailureTracer"/> that writes
    /// failures to a file and logs them too. This file will be deleted at the initialization.
    /// </summary>
    public class SortingGeometryFailureTracer : ISortingGeometryFailureTracer
    {
        #region Private fields

        /// <summary>
        /// The settings of the tracer.
        /// </summary>
        private readonly SortingGeometryFailureTracerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingGeometryFailureTracer"/> class.
        /// </summary>
        /// <param name="settings">The settings of the tracer.</param>
        public SortingGeometryFailureTracer(SortingGeometryFailureTracerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            // Delete the file first
            File.Delete(settings.FailureFilePath);
        }

        #endregion

        #region IConstructorFailureTracer implementation

        /// <inheritdoc/>
        public void TraceInconstructibleObject(RankedTheorem rankedTheorem)
        {
            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                Log.Warning("Problem while drawing objects needed to examine a ranked theorem. See {path} for more detail.", _settings.FailureFilePath);

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FailureFilePath, append: true);

            // Prepare the formatter 
            var rankedTheoremFormatter = new OutputFormatter(rankedTheorem.Configuration.AllObjects);

            // Write initial info
            streamWriter.WriteLine($"Problem while constructing the needed objects to examine:\n");

            // Write the configuration
            streamWriter.WriteLine(rankedTheoremFormatter.FormatConfiguration(rankedTheorem.Configuration));

            // Write the theorem
            streamWriter.WriteLine($"\n{rankedTheoremFormatter.FormatTheorem(rankedTheorem.Theorem)}");

            // Separator
            streamWriter.WriteLine("--------------------------------------------------\n");
        }

        /// <inheritdoc/>
        public void TraceInconstructibleTheorem(RankedTheorem rankedTheorem, AnalyticException exception)
        {
            // If logging is allowed, log it with the reference to more detail in the file
            if (_settings.LogFailures)
                Log.Warning("Problem while drawing a ranked theorem. See {path} for more detail.", _settings.FailureFilePath);

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FailureFilePath, append: true);

            // Prepare the formatter 
            var rankedTheoremFormatter = new OutputFormatter(rankedTheorem.Configuration.AllObjects);

            // Write initial info
            streamWriter.WriteLine($"Problem while constructing the theorem:\n");

            // Write the configuration
            streamWriter.WriteLine(rankedTheoremFormatter.FormatConfiguration(rankedTheorem.Configuration));

            // Write the theorem
            streamWriter.WriteLine($"\n{rankedTheoremFormatter.FormatTheorem(rankedTheorem.Theorem)}");

            // Write the exception
            streamWriter.WriteLine($"\nException: {exception}");

            // Separator
            streamWriter.WriteLine("--------------------------------------------------\n");
        }

        #endregion
    }
}