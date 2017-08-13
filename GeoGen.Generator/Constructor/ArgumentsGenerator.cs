using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.Container;

namespace GeoGen.Generator.Constructor
{
    internal class ArgumentsGenerator : IArgumentsGenerator
    {
        private readonly IConstructionsContainer _constructionsContainer;

        private readonly IConfigurationContainer _configurationContainer;

        public ArgumentsGenerator(IConstructionsContainer constructionsContainer, IConfigurationContainer configurationContainer)
        {
            _constructionsContainer = constructionsContainer;
            _configurationContainer = configurationContainer;
        }

        public IEnumerable<IReadOnlyList<ConstructionArgument>> GenerateArguments(Configuration configuration, Construction construction)
        {
            var objectTypeToObjectsMap = _configurationContainer.GetObjectTypeToObjectsMap(configuration);
            var objectTypeToCountsMap = _constructionsContainer.GetObjectTypeToCountsMap(construction);

            

            return null;
        }
    }
}