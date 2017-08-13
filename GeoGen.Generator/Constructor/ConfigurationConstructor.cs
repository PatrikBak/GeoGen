using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.Constructor
{
    internal class ConfigurationConstructor : IConfigurationConstructor
    {
        private readonly IConstructionsContainer _constructionsContainer;

        private readonly IArgumentsGenerator _argumentsGenerator;

        public ConfigurationConstructor(IConstructionsContainer constructionsContainer, IArgumentsGenerator argumentsGenerator)
        {
            _constructionsContainer = constructionsContainer;
            _argumentsGenerator = argumentsGenerator;
        }

        public IEnumerable<ConstructorOutput> GenerateNewConfigurationObjects(Configuration configuration)
        {
            // Iterate through all constructions
            foreach (var construction in _constructionsContainer)
            {
                // For a given one, iterate through all lists or arguments that can be passed
                // The arguments generator should take care that arguments are unique.
                foreach (var arguments in _argumentsGenerator.GenerateArguments(configuration, construction))
                {
                    // Create output. This looks nicer than passing a tuple of construction and arguments, IMHO.
                    var constructedObject =  new ConstructedConfigurationObject(construction, arguments);

                    yield return new ConstructorOutput(configuration, constructedObject);
                }
            }
        }
    }
}