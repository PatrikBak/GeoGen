using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsConstructor"/>.
    /// </summary>
    internal sealed class ObjectsConstructor : IObjectsConstructor
    {
        #region Private fields

        /// <summary>
        /// The constructions container.
        /// </summary>
        private readonly IConstructionsContainer _constructionsContainer;

        /// <summary>
        /// The arguments generator.
        /// </summary>
        private readonly IArgumentsGenerator _argumentsGenerator;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new objects constructor that takes constructions
        /// from a given constructions container and uses a given arguments
        /// generator to generate arguments that are used to create 
        /// constructed configuration objects.
        /// </summary>
        /// <param name="constructionsContainer">The constructions container.</param>
        /// <param name="argumentsGenerator">The arguments generator.</param>
        public ObjectsConstructor(IConstructionsContainer constructionsContainer, IArgumentsGenerator argumentsGenerator)
        {
            _constructionsContainer = constructionsContainer ?? throw new ArgumentNullException(nameof(constructionsContainer));
            _argumentsGenerator = argumentsGenerator ?? throw new ArgumentNullException(nameof(argumentsGenerator));
        }

        #endregion

        #region IObjectsConstructor methods

        /// <summary>
        /// Performs all possible constructions to a given configuration.
        /// </summary>
        /// <param name="configurationWrapper">The configuration wrapper.</param>
        /// <returns>The enumerable of constructor outputs.</returns>
        public IEnumerable<ConstructorOutput> GenerateOutput(ConfigurationWrapper configurationWrapper)
        {
            if (configurationWrapper == null)
                throw new ArgumentNullException(nameof(configurationWrapper));

            // For each construction create many outputs
            return _constructionsContainer.SelectMany
            (
                // For a single construction
                constructionWrapper =>
                {
                    // Generate arguments
                    var generatedArguments = _argumentsGenerator.GenerateArguments(configurationWrapper, constructionWrapper);

                    // Unwrap construction
                    var unwrapedConstruction = constructionWrapper.Construction;

                    // Create new output enumerable
                    var newOutput = generatedArguments.Select
                    (
                        arguments =>
                        {
                            // Construct objects  with all indices
                            var constructedObjects = Enumerable.Range(0, unwrapedConstruction.OutputTypes.Count)
                                    .Select(i => new ConstructedConfigurationObject(unwrapedConstruction, arguments, i))
                                    .ToList();

                            // Create and return an output for these objects
                            return new ConstructorOutput
                            {
                                OriginalConfiguration = configurationWrapper,
                                ConstructedObjects = constructedObjects
                            };
                        }
                    );

                    // Return it
                    return newOutput;
                });
        }

        #endregion
    }
}