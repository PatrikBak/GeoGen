using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly ICombinator<ConfigurationObjectType, IEnumerator<ConfigurationObject>> _combinator;

        public ArgumentsGenerator(ICombinator<ConfigurationObjectType, IEnumerator<ConfigurationObject>> combinator,
                                  IConstructionSignatureMatcher constructionSignatureMatcher,
                                  IVariationsProvider<ConfigurationObject> variationsProvider,
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

            var s2 = new Stopwatch();
            s2.Start();

            var dictionaryForCombinator = configuration.ObjectTypeToObjects.Where
            (
                pair => construction.ObjectTypesToNeededCount.ContainsKey(pair.Key)
            ).
            ToDictionary
            (
                keyValue => keyValue.Key,
                keyValue => _variationsProvider.GetVariations(keyValue.Value, construction.ObjectTypesToNeededCount[keyValue.Key])
                                               .Select(variation => variation.GetEnumerator())
            );
            s2.Stop();
            Console.WriteLine($"Variations: {s2.ElapsedMilliseconds}");

            _argumentsContainer.Clear();

            var stopWatch = new Stopwatch();
            var matchingTime = 0L;
            var calls = 0;
            var s = new Stopwatch();
            var m2 = 0L;

            foreach (var dictonaryForMatcher in _combinator.Combine(dictionaryForCombinator))
            {
                calls++;
                stopWatch.Start();
                _constructionSignatureMatcher.Initialize(dictonaryForMatcher);

                var parameters = construction.Construction.ConstructionParameters;

                var arguments = _constructionSignatureMatcher.Match(parameters);
                matchingTime += stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();

                s.Start();
                _argumentsContainer.Add(arguments);
                m2 += s.ElapsedMilliseconds;
                s.Reset();
            }

            var hm =  _argumentsContainer.ToList();

            Console.WriteLine($"Calls: {calls}");
            Console.WriteLine($"Matching: {matchingTime}");
            Console.WriteLine($"Container adding: {m2}");


            return hm;
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