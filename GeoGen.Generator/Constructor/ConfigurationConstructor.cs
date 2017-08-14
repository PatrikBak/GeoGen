using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.Constructor.Arguments;
using GeoGen.Generator.Constructor.Container;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Constructor
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
                // For a given one, iterate through all lists or arguments that can be passed
                // The arguments generator should take care that arguments are unique.
                foreach (var arguments in _argumentsGenerator.GenerateArguments(configuration, construction))
                {
                    // Create output. This looks nicer than passing a tuple of construction and arguments, IMHO.
                    var constructedObject = new ConstructedConfigurationObject(construction.Construction, arguments);

                    yield return new ConstructorOutput(configuration, constructedObject);
                }
            }
        }
    }
}