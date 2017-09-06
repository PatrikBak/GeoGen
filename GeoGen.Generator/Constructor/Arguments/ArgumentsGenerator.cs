using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities.Combinator;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.Constructor.Arguments.Container;
using GeoGen.Generator.Constructor.Arguments.SignatureMatching;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Constructor.Arguments
{
    internal class ArgumentsGenerator : IArgumentsGenerator
    {
        private readonly IConstructionSignatureMatcher _constructionSignatureMatcher;

        private readonly IArgumentsContainer _argumentsContainer;

        private readonly IVariationsProvider<ConfigurationObject> _variationsProvider;

        private readonly ICombinator<ConfigurationObjectType, List<ConfigurationObject>> _combinator;

        public ArgumentsGenerator(ICombinator<ConfigurationObjectType, List<ConfigurationObject>> combinator,
            IConstructionSignatureMatcher constructionSignatureMatcher, IVariationsProvider<ConfigurationObject> variationsProvider,
            IArgumentsContainer argumentsContainer)
        {
            _constructionSignatureMatcher = constructionSignatureMatcher;
            _argumentsContainer = argumentsContainer;
            _variationsProvider = variationsProvider;
            _combinator = combinator;
        }

        public IEnumerable<IReadOnlyList<ConstructionArgument>> GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction)
        {
            // First we check if we can even perform the construction. Whether there are enough
            // objects to do so
            if (!CanWePerformConstruction(configuration, construction))
                return Enumerable.Empty<IReadOnlyList<ConstructionArgument>>();

            var dictionaryForCombinator = configuration.ObjectTypeToObjects.Where
                    (
                        pair => construction.ObjectTypesToNeededCount.ContainsKey(pair.Key)
                    ).ToDictionary
                    (
                        keyValue => keyValue.Key,
                        keyValue => _variationsProvider
                                .GetVariations(keyValue.Value, construction.ObjectTypesToNeededCount[keyValue.Key])
                                .Select(variation => variation.ToList())
                    );

            _argumentsContainer.Clear();

            foreach (var dictonaryForMatcher in _combinator.Combine(dictionaryForCombinator))
            {
                _constructionSignatureMatcher.Initialize(dictonaryForMatcher);

                var parameters = construction.Construction.ConstructionParameters;

                var arguments = _constructionSignatureMatcher.Match(parameters);

                _argumentsContainer.Add(arguments);
            }

            return _argumentsContainer;
        }

        /// <summary>
        /// Returns if we can perform the currently examined construction according to
        /// the given dictonaries.
        /// </summary>
        /// <returns>true, if there is no object type (such as Point) from which we have fewer
        /// objects than we need to construction.</returns>
        private static bool CanWePerformConstruction(ConfigurationWrapper configuration, ConstructionWrapper construction)
        {
            foreach (var pair in construction.ObjectTypesToNeededCount)
            {
                var numberOfElementsInArguments = pair.Value;

                if (!configuration.ObjectTypeToObjects.ContainsKey(pair.Key))
                    return false;

                var realObjects = configuration.ObjectTypeToObjects[pair.Key];
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