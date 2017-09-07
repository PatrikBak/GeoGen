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
    internal class ConfigurationConstructor : IConfigurationConstructor
    {
        private readonly IConstructionsContainer _constructionsContainer;

        private readonly IArgumentsGenerator _argumentsGenerator;

        public ConfigurationConstructor(IConstructionsContainer constructionsContainer, IArgumentsGenerator argumentsGenerator)
        {
            _constructionsContainer = constructionsContainer ?? throw new ArgumentNullException(nameof(argumentsGenerator));
            _argumentsGenerator = argumentsGenerator ?? throw new ArgumentNullException(nameof(argumentsGenerator));
        }

        public IEnumerable<ConstructorOutput> GenerateNewConfigurationObjects(ConfigurationWrapper configuration)
        {
            // Iterate through all constructions
            foreach (var construction in _constructionsContainer)
            {
                // Take the forbidden arguments for this construction form the configuration wrapper
                var constructionId = construction.Construction.Id;
                var forbiddenArguments = configuration.ConstructionIdToForbiddenArguments;

                // A local helper function to check if given arguments should be proccessed further
                bool ArgumentsAreNotForbidden(IReadOnlyList<ConstructionArgument> arguments)
                {
                    return !forbiddenArguments.ContainsKey(constructionId) || forbiddenArguments[constructionId].Contains(arguments);
                }

                // Generate new output
                var newOutput = _argumentsGenerator // Generate arguments
                        .GenerateArguments(configuration, construction) // That are not forbidden
                        .Where(ArgumentsAreNotForbidden) // Cast them to the construction output
                        .Select
                        (
                            arguments =>
                            {
                                var unwrapedConstruction = construction.Construction;

                                // Construct objects 
                                var constructedObjects = Enumerable.Range(0, unwrapedConstruction.OutputTypes.Count)
                                        .Select(i => new ConstructedConfigurationObject(unwrapedConstruction, arguments, i))
                                        .ToList();

                                // Create and return an output for these objects
                                return new ConstructorOutput(configuration, constructedObjects);
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
    }
}