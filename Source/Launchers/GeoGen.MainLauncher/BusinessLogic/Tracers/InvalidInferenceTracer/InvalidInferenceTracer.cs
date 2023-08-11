using GeoGen.Core;
using GeoGen.TheoremProver;
using GeoGen.Utilities;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The implementation of <see cref="IInvalidInferenceTracer"/> that writes invalid inferences to a folder
    /// where each <see cref="InferenceRule"/> will have a custom file with all of its incorrect inferences.
    /// All files of the folder will be deleted at the beginning.
    /// </summary>
    public class InvalidInferenceTracer : IInvalidInferenceTracer
    {
        #region Private fields

        /// <summary>
        /// The settings of the tracer.
        /// </summary>
        private readonly InvalidInferenceTracerSettings _settings;

        /// <summary>
        /// The dictionary that keeps track of the number of already written invalid inferences in particular files.
        /// </summary>
        private readonly Dictionary<string, int> _invalidInferencesPerFile = new Dictionary<string, int>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInferenceTracer"/> class.
        /// </summary>
        /// <param name="settings">The settings of the tracer.</param>
        public InvalidInferenceTracer(InvalidInferenceTracerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            // Delete all the files of the folder
            Directory.EnumerateFiles(_settings.InvalidInferenceFolder).ForEach(File.Delete);
        }

        #endregion

        #region IInvalidInferenceTracer implementation

        /// <inheritdoc/>
        public void MarkInvalidInferrence(Configuration configuration, Theorem invalidConclusion, InferenceRule inferenceRule, Theorem[] negativeAssumptions, Theorem[] possitiveAssumptions)
        {
            // Prepare the file path for the rule with the name of the rule
            var filePath = Path.Combine(_settings.InvalidInferenceFolder, $"{inferenceRule.ToString().Replace(Path.DirectorySeparatorChar, '_')}.{_settings.FileExtension}");

            // If adding this inference would reach the maximal number of written inferences, we're done
            if (_invalidInferencesPerFile.GetValueOrDefault(filePath) + 1 > _settings.MaximalNumberOfInvalidInferencesPerFile)
                return;

            // Otherwise create or get the file in the invalid inference folder 
            using var writer = new StreamWriter(filePath, append: true);

            // Prepare the formatter of the configuration
            var formatter = new OutputFormatter(configuration.AllObjects);

            // Write the configuration
            writer.WriteLine(formatter.FormatConfiguration(configuration));

            // An empty line
            writer.WriteLine();

            // Write the incorrect theorem
            writer.WriteLine($" {formatter.FormatTheorem(invalidConclusion)}");

            // Write its assumptions
            possitiveAssumptions.ForEach(assumption => writer.WriteLine($"  - {formatter.FormatTheorem(assumption)}"));

            // As well as negative ones
            negativeAssumptions.ForEach(assumption => writer.WriteLine($"  ! {formatter.FormatTheorem(assumption)}"));

            // Separator
            writer.WriteLine("--------------------------------------------------\n");

            // Mark that we've used this inference
            _invalidInferencesPerFile[filePath] = _invalidInferencesPerFile.GetValueOrDefault(filePath) + 1;
        }

        #endregion
    }
}
