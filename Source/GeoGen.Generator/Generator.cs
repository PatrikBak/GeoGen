using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IGenerator"/>. It generates configurations by layers.
    /// Initially the first layer consists of the initial configuration from the <see cref="GeneratorInput"/>.
    /// Then in every iteration we extend all the configurations from the current layer with new 
    /// <see cref="ConstructedConfigurationObject"/>, that are created using <see cref="IArgumentsGenerator"/>,
    /// and the <see cref="Construction"/>s/ from the input. We use <see cref="IContainer{T}{T}"/>, where 'T' 
    /// is <see cref="ConfigurationObject"/>, to recognize equal objects. Then we validate each created 
    /// configuration using  <see cref="IConfigurationsValidator"/>. The valid configurations represent the
    /// next layer. Every valid configuration is then analyzer for theorems using <see cref="ITheoremsAnalyzer"/>. 
    /// The generation process is lazy. Once it has started, we can't run it again with the same instance of the class.
    /// </summary>
    public class Generator : IGenerator
    {
        #region Dependencies

        /// <summary>
        /// The container of configuration objects (initially empty) that recognizes equal objects.
        /// </summary>
        private readonly IContainer<ConfigurationObject> _container;

        /// <summary>
        /// The generator of arguments that are passed to newly created constructed objects.
        /// </summary>
        private readonly IArgumentsGenerator _generator;

        /// <summary>
        /// The validator of newly generated configurations.
        /// </summary>
        private readonly IConfigurationsValidator _validator;

        /// <summary>
        /// The analyzer of theorems in the generated configurations.
        /// </summary>
        private readonly ITheoremsAnalyzer _analyzer;

        #endregion

        #region Private fields

        /// <summary>
        /// The input for the generator.
        /// </summary>
        private readonly GeneratorInput _input;

        /// <summary>
        /// The configurations that are to be extended
        /// </summary>
        private List<GeneratedConfiguration> _currentLayer;

        /// <summary>
        /// Indicates if the generation process has started yet. 
        /// </summary>
        private bool _generationStarted;

        /// <summary>
        /// The id prepared to be set to the next generated configuration object.
        /// </summary>
        private int _nextConfigurationObjectId;

        /// <summary>
        /// The id prepared to be set to the next generated configuration.
        /// </summary>
        private int _nextConfigurationId;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="input">The input for the generator.</param>
        /// <param name="container">The container of configuration objects (initially empty) that recognizes equal objects.</param>
        /// <param name="generator">The generator of arguments that are passed to newly created constructed objects.</param>
        /// <param name="validator">The validator of newly generated configurations.</param>
        /// <param name="analyzer">The analyzer of theorems in the generated configurations.</param>
        public Generator(GeneratorInput input, IContainer<ConfigurationObject> container, IArgumentsGenerator generator, IConfigurationsValidator validator, ITheoremsAnalyzer analyzer)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
        }

        #endregion

        #region IGenerator implementation

        /// <summary>
        /// Starts the generation process and lazily returns the output. The algorithm is described
        /// in the documentation of this class.
        /// </summary>
        /// <returns>A lazy enumerable of generator outputs.</returns>
        public IEnumerable<GeneratorOutput> Generate()
        {
            // Make sure the generator is not re-used
            if (_generationStarted)
                throw new GeneratorException("The generator cannot be used twice. Create a new one.");

            // Mark that we've started
            _generationStarted = true;

            #region Initialize constructions

            // Make sure the constructions have distinct names
            if (_input.Constructions.Select(construction => construction.Name).Count() != _input.Constructions.Count)
                throw new InitializationException("The constructions are supposed to have mutually distinct names");

            // Identify them
            _input.Constructions.ForEach((construction, index) => construction.Id = index);

            #endregion

            #region Initialize the initial objects

            // For each object...
            _input.InitialConfiguration.ObjectsMap.AllObjects.ForEach(obj =>
            {
                // Make sure it is identified
                obj.Id = _nextConfigurationObjectId++;

                // And added to the container
                _container.Add(obj);
            });

            #endregion

            #region Initialize the first layer

            // Create the initial generated configuration from the passed initial one
            var firstConfiguration = new GeneratedConfiguration(_input.InitialConfiguration, _nextConfigurationId++);

            // Make sure it's valid
            if (!_validator.Validate(firstConfiguration))
                throw new InitializationException("The initial configuration is not valid");

            // At this point it is valid. Create the first layer
            _currentLayer = new List<GeneratedConfiguration> { firstConfiguration };

            #endregion

            #region Algorithm

            // Execute the requested number of iterations
            for (var iterationIndex = 0; iterationIndex < _input.MaximalNumberOfIterations; iterationIndex++)
            {
                // In each iteration we initialize a list for new configurations that will be extended
                // in the next iteration. We're gonna fill it with our the configurations generated
                // from the current layer
                var generatedConfigurations = new List<GeneratedConfiguration>();

                // Prepare the enumerable that performs the algorithm that takes a configuration
                // and extends it with new objects. Each configuration will be projected into one or more generator outputs
                var generatedOutputEnumerable = _currentLayer.SelectMany(currentConfiguration =>
                {
                    // For a given configuration we create all possible objects that are use our constructions
                    return _input.Constructions.SelectMany(construction =>
                    {
                        // For a given construction first generate all possible arguments
                        var generatedArguments = _generator.GenerateArguments(currentConfiguration.ObjectsMap, construction.Signature);

                        // Cast each arguments to a new constructed object
                        return generatedArguments.Select(arguments => new ConstructedConfigurationObject(construction, arguments, _nextConfigurationObjectId++));
                    })
                    // Make sure each constructed object are added to the container so we can recognize equal ones
                    .Select(newObject =>
                    {
                        // Add the new object to the container. It will found out if there 
                        // already is an equal version of it inside
                        _container.TryAdd(newObject, out var equalObject);

                        // If there is an equal object already in the container
                        if (equalObject != null)
                        {
                            // Then we'll re-assign the value to be returned further
                            // This also step makes sure that equal objects won't be 
                            // used with more than one instance. The objects that we 
                            // will re-assign shouldn't be used elsewhere and thus 
                            // will be eventually eaten by the garbage collector. 
                            newObject = (ConstructedConfigurationObject) equalObject;
                        }

                        // Return the new object
                        return newObject;
                    })
                    // For each new object create a new generated configuration
                    .Select(newObject => new GeneratedConfiguration(currentConfiguration, newObject, _nextConfigurationId++))
                    // Take only valid ones
                    .Where(_validator.Validate)
                    // Finally construct the output for each of them
                    .Select(generatedConfiguration =>
                    {
                        // Run the analyzer
                        var analyzerOutput = _analyzer.Analyze(generatedConfiguration);

                        // Make sure it's added to the generated list 
                        // if the configuration was analyzed properly
                        // If not, we then there is no point in extending it
                        if (analyzerOutput.TheoremAnalysisSuccessful)
                            generatedConfigurations.Add(generatedConfiguration);

                        // Construct the output
                        return new GeneratorOutput
                        {
                            // Set the configuration
                            Configuration = generatedConfiguration,

                            // Set the output
                            AnalyzerOutput = analyzerOutput
                        };
                    });
                });

                // Perform the generation by enumerating the enumerable 
                // and lazily return the output. This generation will
                // gradually fill the 'generatedConfigurations' list
                foreach (var output in generatedOutputEnumerable)
                    yield return output;

                // At this point the next layer is prepared
                // We can set our generated configurations to be the next layer
                _currentLayer = new List<GeneratedConfiguration>(generatedConfigurations);
            }

            #endregion
        }

        #endregion
    }
}