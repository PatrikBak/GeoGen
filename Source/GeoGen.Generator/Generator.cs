using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implemention of <see cref="IGenerator"/>.
    /// </summary>
    public class Generator : IGenerator
    {
        #region Dependencies

        /// <summary>
        /// The constructor of new objects that should be added to configurations.
        /// </summary>
        private readonly IObjectsGenerator _constructor;

        /// <summary>
        /// The validator of newnly generated configurations.
        /// </summary>
        private readonly IConfigurationsValidator _validator;

        /// <summary>
        /// The analyzer of theorems in the generated configurations.
        /// </summary>
        private readonly ITheoremsAnalyzer _analyzer;

        #endregion

        #region Private fields

        /// <summary>
        /// The configurations that are to be extended
        /// </summary>
        private List<GeneratedConfiguration> _currentLayer;

        /// <summary>
        /// The id that is prepared for a new configuration.
        /// </summary>
        private int _currentConfigurationId;

        /// <summary>
        /// The input for the generator.
        /// </summary>
        private readonly GeneratorInput _input;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="input">The input for the generator.</param>
        /// <param name="generator">The generator of new objects that should be added to configurations.</param>
        /// <param name="validator">The validator of newnly generated configurations.</param>
        /// <param name="analyzer">The analyzer of theorems in the generated configurations.</param>
        public Generator(GeneratorInput input, IObjectsGenerator generator, IConfigurationsValidator validator, ITheoremsAnalyzer analyzer)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _constructor = generator ?? throw new ArgumentNullException(nameof(generator));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
        }

        #endregion

        #region IGenerator implementation

        /// <summary>
        /// Starts the generation process and lazily returns the output.
        /// </summary>
        /// <returns>The generator output enumerable.</returns>
        public IEnumerable<GeneratorOutput> Generate()
        {
            #region Initialization

            // Create the first configuration
            var firstConfiguration = new GeneratedConfiguration(_input.InitialConfiguration);

            // Make sure it's valid
            if (!_validator.Validate(firstConfiguration))
                throw new GeneratorException("The first configuration is not valid");

            // Create the first layer
            _currentLayer = new List<GeneratedConfiguration> { firstConfiguration };

            #endregion

            #region Algorithm

            // Executed the requested number of iterations
            for (var iterationIndex = 0; iterationIndex < _input.MaximalNumberOfIterations; iterationIndex++)
            {
                // In each iteration we inicialize a list for new configurations that will be extended
                // in the next iteration. We're gonna fill it with our the configurations generated
                // from the current layer
                var generatedConfigurations = new List<GeneratedConfiguration>();

                // Prepare the enumerable that performs the algorithm that takes a configuration
                // and extends it with new new objects. Each configuration will be projected into one or more generator outputs
                var generatedOutputEnumerable = _currentLayer.SelectMany(currentConfiguration =>
                {
                    // For a given configuration we take all possible constructed objects
                    return _constructor.ConstructPossibleObjects(currentConfiguration)
                        // Project each option into a generator output, if it turns out to be 
                        // a valid one, or null, if not. We ignore the null ones
                        .SelectNotNull(newObjects =>
                        {
                            // Construct the new generated configuration
                            var generatedConfiguration = new GeneratedConfiguration(currentConfiguration, newObjects);

                            // Validate it. If it's not valid...
                            if (!_validator.Validate(generatedConfiguration))
                                // Return null and make it ignored because of the 'SelectNotNull' method
                                return null;

                            // Otherwise it's valid...Make sure it's added to the generated list
                            generatedConfigurations.Add(generatedConfiguration);

                            // Construct the output
                            return new GeneratorOutput
                            {
                                // Set the configuration
                                Configuration = generatedConfiguration,

                                // Find theorems
                                Theorems = _analyzer.Analyze(generatedConfiguration)
                            };
                        });
                });

                // Perform the generation and lazily return the output
                // This generation will gradully fill the 'generatedConfigurations' list
                foreach (var output in generatedOutputEnumerable)
                    yield return output;

                // At this point the next layer is prepared
                // We can set our generated configurations to be the new layer
                _currentLayer = new List<GeneratedConfiguration>(generatedConfigurations);

            }

            #endregion
        }

        #endregion
    }
}