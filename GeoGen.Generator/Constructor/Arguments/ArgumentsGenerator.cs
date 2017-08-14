using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.Constructor.Arguments.Container;
using GeoGen.Generator.Constructor.Arguments.SignatureMatching;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Constructor.Arguments
{
    internal class ArgumentsGenerator : IArgumentsGenerator
    {
        private readonly ISignaturesCombinator _signaturesCombinator;

        private readonly IConfigurationObjectsIterator _configurationObjectsIterator;

        private readonly IConstructionSignatureMatcher _constructionSignatureMatcher;

        private readonly IArgumentsContainer _argumentsContainer;

        public ArgumentsGenerator(ISignaturesCombinator signaturesCombinator, IConfigurationObjectsIterator configurationObjectsIterator,
            IConstructionSignatureMatcher constructionSignatureMatcher, IArgumentsContainer argumentsContainer)
        {
            _signaturesCombinator = signaturesCombinator;
            _configurationObjectsIterator = configurationObjectsIterator;
            _constructionSignatureMatcher = constructionSignatureMatcher;
            _argumentsContainer = argumentsContainer;
        }

        public IEnumerable<IReadOnlyList<ConstructionArgument>> GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction)
        {
            if (!CanWePerformConstruction(configuration, construction))
                return Enumerable.Empty<IReadOnlyList<ConstructionArgument>>();

            var parameters = construction.Construction.ConstructionParameters;
            _argumentsContainer.Clear();

            foreach (var dictonaryForMatcher in _signaturesCombinator.Combine(configuration, construction))
            {
                _configurationObjectsIterator.Initialize(dictonaryForMatcher);

                var arguments = _constructionSignatureMatcher.Match(_configurationObjectsIterator, parameters);

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