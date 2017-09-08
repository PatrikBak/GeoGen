using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling;
using GeoGen.Generator.Constructing.Arguments;
using GeoGen.Generator.Constructing.Container;

namespace GeoGen.Generator.Constructing
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationConstructor"/>.
    /// </summary>
    internal class ConfigurationConstructor : IConfigurationConstructor
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
        public ConfigurationConstructor(IConstructionsContainer constructionsContainer, IArgumentsGenerator argumentsGenerator)
        {
            _constructionsContainer = constructionsContainer ?? throw new ArgumentNullException(nameof(argumentsGenerator));
            _argumentsGenerator = argumentsGenerator ?? throw new ArgumentNullException(nameof(argumentsGenerator));
        }

        #endregion

        #region IConfigurationConstructor methods

        /// <summary>
        /// Performs all possible constructions to a given configution wrapper.
        /// </summary>
        /// <param name="configurationWrapper">The given configuration wrapper.</param>
        /// <returns>The enumerable of constructor output.</returns>
        public IEnumerable<ConstructorOutput> GenerateNewConfigurationObjects(ConfigurationWrapper configurationWrapper)
        {
            // Iterate through all constructions
            foreach (var constructionWrapper in _constructionsContainer)
            {
                // A local helper function to check if given arguments should be proccessed further
                bool ArgumentsAreNotForbidden(IReadOnlyList<ConstructionArgument> arguments)
                {
                    var constructionId = constructionWrapper.Construction.Id;
                    var forbiddenArguments = configurationWrapper.ConstructionIdToForbiddenArguments;

                    return !forbiddenArguments.ContainsKey(constructionId) || forbiddenArguments[constructionId].Contains(arguments);
                }

                // Create new output enumerale
                var newOutput = _argumentsGenerator
                        // Generate arguments
                        .GenerateArguments(configurationWrapper, constructionWrapper)
                        // That are not forbidden
                        .Where(ArgumentsAreNotForbidden)
                        // Cast them to the construction output
                        .Select
                        (
                            arguments =>
                            {
                                // Unwrap construction
                                var unwrapedConstruction = constructionWrapper.Construction;

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

                // Lazily iterate through the enumerable
                // TODO: Compare lazy and non-lazy approach (currently it's enumerated instantly)
                foreach (var constructorOutput in newOutput)
                {
                    yield return constructorOutput;
                }
            }
        }

        #endregion
    }
}