using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer;
using GeoGen.Generator.Constructing.Arguments;
using GeoGen.Generator.Constructing.Container;

namespace GeoGen.Generator.Constructing
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsConstructor"/>.
    /// </summary>
    internal class ObjectsConstructor : IObjectsConstructor
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
        /// Constructs a new configuration construction from a construction containerand an arguments generator.
        /// </summary>
        /// <param name="constructionsContainer">The construction container.</param>
        /// <param name="argumentsGenerator">The arguments generator.</param>
        public ObjectsConstructor(IConstructionsContainer constructionsContainer, IArgumentsGenerator argumentsGenerator)
        {
            _constructionsContainer = constructionsContainer ?? throw new ArgumentNullException(nameof(argumentsGenerator));
            _argumentsGenerator = argumentsGenerator ?? throw new ArgumentNullException(nameof(argumentsGenerator));
        }

        #endregion

        #region IObjectsConstructor methods

        /// <summary>
        /// Performs all possible constructions to a given configution wrapper.
        /// </summary>
        /// <param name="configurationWrapper">The given configuration wrapper.</param>
        /// <returns>The enumerable of constructor output.</returns>
        public IEnumerable<ConstructorOutput> GenerateNewConfigurationObjects(ConfigurationWrapper configurationWrapper)
        {
            if (configurationWrapper == null)
                throw new ArgumentNullException(nameof(configurationWrapper));

            return _constructionsContainer.SelectMany
            (
                constructionWrapper =>
                {
                    // Generate arguments
                    var generatedArguments = _argumentsGenerator.GenerateArguments(configurationWrapper, constructionWrapper);

                    // Pull forbidden arguments for the current constrution
                    var forbiddenArguments = configurationWrapper.ForbiddenArguments;

                    // Unwrap construction
                    var unwrapedConstruction = constructionWrapper.Construction;

                    // Pull the construction id.
                    var constructionId = unwrapedConstruction.Id ?? throw new GeneratorException();

                    // If there are any arguments to be excluded, exclude them.
                    if (forbiddenArguments.ContainsKey(constructionId))
                    {
                        generatedArguments.RemoveElementsFrom(forbiddenArguments[constructionId]);
                    }

                    // Create new output enumerale
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
                                InitialConfiguration = configurationWrapper,
                                ConstructedObjects = constructedObjects
                            };
                        }
                    );

                    return newOutput;
                });
        }

        #endregion
    }
}