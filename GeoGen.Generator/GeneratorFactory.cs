using GeoGen.Core.Generator;
using GeoGen.Generator.Constructor;
using GeoGen.Generator.Constructor.Arguments;
using GeoGen.Generator.Container;
using GeoGen.Generator.Handler;

namespace GeoGen.Generator
{
    internal class GeneratorFactory : IGeneratorFactory
    {
        public IGenerator CreateGenerator(GeneratorInput generatorInput)
        {
            var container = new ConfigurationContainer(generatorInput.InitialConfiguration);
            var constructionsContainer = new ConstructionsContainer();
            var argumentsGenerator = new ArgumentsGenerator(constructionsContainer, container, null, null);
            var constructions = new ConfigurationConstructor(constructionsContainer, argumentsGenerator);
            var handler = new ConfigurationsHandler(container, constructions);
            var generatorContext = new GeneratorContext(container, handler, constructions);

            return new Generator(generatorContext, generatorInput.MaximalNumberOfIterations);
        }
    }
}