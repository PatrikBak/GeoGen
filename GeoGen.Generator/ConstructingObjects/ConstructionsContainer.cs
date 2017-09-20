using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator.ConstructingObjects
{
    /// <summary>
    /// A default implementation of <see cref="IConstructionsContainer"/>.
    /// 
    /// TODO: Initialization
    /// </summary>
    internal class ConstructionsContainer : IConstructionsContainer
    {
        #region Private fields

        /// <summary>
        /// The constructions list
        /// </summary>
        private readonly List<ConstructionWrapper> _constructions = new List<ConstructionWrapper>();

        #endregion

        #region IConstructions container methods

        /// <summary>
        /// Initializes the container with a given enumerable of constructions.
        /// It performs the check whether the constructions have distinct ids (which is needed
        /// during the generation process).
        /// </summary>
        /// <param name="constructions"></param>
        public void Initialize(IEnumerable<Construction> constructions)
        {
            if (constructions == null)
                throw new ArgumentNullException(nameof(constructions));

            // Enumerate
            var constructionsList = constructions.ToList();

            // Check distint ids
            if (constructionsList.Select(c => c.Id).ToSet().Count != constructionsList.Count)
                throw new ArgumentException("Passed constructions don't have unique ids.");

            // Set new items to the container
            _constructions.SetItems
            (
                constructionsList.Select
                (
                    construction => new ConstructionWrapper
                    {
                        Construction = construction,
                        ObjectTypesToNeededCount = DetermineObjectTypesToCount(construction)
                    }
                )
            );
        }

        /// <summary>
        /// Gets the type to types count directionary from a given construction.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <returns>The configuration object type to count dictionary.</returns>
        private static IReadOnlyDictionary<ConfigurationObjectType, int> DetermineObjectTypesToCount(Construction construction)
        {
            var allTypes = construction.ConstructionParameters.SelectMany(ObjectTypesOf);

            var result = new Dictionary<ConfigurationObjectType, int>();

            foreach (var type in allTypes)
            {
                if (result.ContainsKey(type))
                {
                    result[type]++;
                }
                else
                {
                    result.Add(type, 1);
                }
            }

            return result;
        }

        /// <summary>
        /// A recursive method to determine the list of types contained within a single
        /// construction parameter.
        /// </summary>
        /// <param name="constructionParameter">The construction parameter.</param>
        /// <returns>The types contained within it.</returns>
        private static List<ConfigurationObjectType> ObjectTypesOf(ConstructionParameter constructionParameter)
        {
            if (constructionParameter is ObjectConstructionParameter objectConstructionParameter)
                return new List<ConfigurationObjectType> {objectConstructionParameter.ExpectedType};

            var setParameter = constructionParameter as SetConstructionParameter ?? throw new NullReferenceException();
            var innerTypes = ObjectTypesOf(setParameter.TypeOfParameters);

            return Enumerable.Range(0, setParameter.NumberOfParameters)
                    .SelectMany(i => innerTypes)
                    .ToList();
        }

        #endregion

        #region IEnumerable methods

        public IEnumerator<ConstructionWrapper> GetEnumerator()
        {
            return _constructions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}