using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.Constructor.Arguments.Container;
using GeoGen.Generator.Container;

namespace GeoGen.Generator.Constructor.Arguments
{
    internal class ArgumentsGenerator : IArgumentsGenerator
    {
        private readonly IConstructionsContainer _constructionsContainer;

        private readonly IConfigurationContainer _configurationContainer;

        private readonly IArgumentsContainer _argumentsContainer;

        private readonly IVariationsProvider _variationsProvider;

        public ArgumentsGenerator(IConstructionsContainer constructionsContainer, IConfigurationContainer configurationContainer, 
            IArgumentsContainer argumentsContainer, IVariationsProvider variationsProvider)
        {
            _constructionsContainer = constructionsContainer;
            _configurationContainer = configurationContainer;
            _argumentsContainer = argumentsContainer;
            _variationsProvider = variationsProvider;
        }

        public IEnumerable<IReadOnlyList<ConstructionArgument>> GenerateArguments(Configuration configuration, Construction construction)
        {
            var objectTypeToObjectsMap = _configurationContainer.GetObjectTypeToObjectsMap(configuration);
            var objectTypeToCountsMap = _constructionsContainer.GetObjectTypeToCountsMap(construction);

            if (!CanWePerformConstruction(objectTypeToCountsMap, objectTypeToObjectsMap))
                return Enumerable.Empty<IReadOnlyList<ConstructionArgument>>();

            

            return null;
        }

        /// <summary>
        /// Returns if we can perform the currently examined construction according to
        /// the given dictonaries.
        /// </summary>
        /// <param name="objectTypeToCountsMap">The object type to needeed count dictionary.</param>
        /// <param name="objectTypeToObjectsMap">The object type to all objects of given type dictionary.</param>
        /// <returns>true, if there is no object type (such as Point) from which we have fewer
        /// objects than we need to construction.</returns>
        private static bool CanWePerformConstruction(IReadOnlyDictionary<ConfigurationObjectType, int> objectTypeToCountsMap, 
            IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> objectTypeToObjectsMap)
        {
            foreach (var pair in objectTypeToCountsMap)
            {
                var numberOfElementsInArguments = pair.Value;
                var realObjects = objectTypeToObjectsMap[pair.Key];
                var numberOfRealObjects = realObjects.Count;

                // if there are more neeed arguments than available objects, 
                // then we can't perform the construction
                if (numberOfElementsInArguments > numberOfRealObjects)
                    return false;
            }

            return true;
        }
    }
}