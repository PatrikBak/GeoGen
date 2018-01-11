using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Generator;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConstructionsContainer"/>.
    /// It has the constructor that expects initial constructions to be passed in.
    /// The <see cref="ConstructionWrapper"/>s will be then created from the them.
    /// </summary>
    internal class ConstructionsContainer : IConstructionsContainer
    {
        #region Private fields

        /// <summary>
        /// The list containing all construction wrappers
        /// </summary>
        private readonly List<ConstructionWrapper> _constructions = new List<ConstructionWrapper>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a container that contains given constructions.
        /// </summary>
        /// <param name="constructions">The constructions.</param>
        public ConstructionsContainer(IEnumerable<Construction> constructions)
        {
            Initialize(constructions);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the container with a given enumerable of constructions.
        /// The ids of the constructions must have their ids set.
        /// </summary>
        /// <param name="constructions">The constructions enumerable.</param>
        private void Initialize(IEnumerable<Construction> constructions)
        {
            if (constructions == null)
                throw ConstructionException.ConstructionsMustNotBeNull();

            // Enumerate the constructions
            var constructionsList = constructions.ToList();

            // Make sure that all constructions have unique ids
            CheckUniqueIds(constructionsList);

            // Set new items to the container casted to the construction wrapper
            _constructions.SetItems(constructionsList.Select(construction => new ConstructionWrapper
            {
                WrappedConstruction = construction,
                ObjectTypesToNeededCount = DetermineObjectTypesToCount(construction)
            }));
        }

        /// <summary>
        /// Checks if given constructions have unique ids.
        /// </summary>
        /// <param name="constructions">The constructions.</param>
        private static void CheckUniqueIds(IReadOnlyCollection<Construction> constructions)
        {
            // Cast constructions to the set of their ids (and check for null constructions on go)
            var ids = constructions.Select(construction => construction ?? throw ConstructionException.ConstructionMostNotBeNull())
                    .Select(construction => construction.Id ?? throw ConstructionException.ConstructionIdNotSet())
                    .ToSet();

            // The ids are unique if and only if the number of actual ids is the same
            // as the number of the constructions
            var uniqueIds = ids.Count == constructions.Count;

            // We expect ids to be unique
            if (!uniqueIds)
                throw ConstructionException.ConstructionsMustHaveUniqueId();
        }

        /// <summary>
        /// Gets a dictionary mapping object types to number of objects of that type needed
        /// to be passed to a given construction.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <returns>The dictionary.</returns>
        private Dictionary<ConfigurationObjectType, int> DetermineObjectTypesToCount(Construction construction)
        {
            // Get the list containing all needed types (with possibly duplicate items)
            var allTypes = construction.ConstructionParameters.SelectMany(ObjectTypesOf);

            // Initialize the resulting dictionary
            var result = new Dictionary<ConfigurationObjectType, int>();

            // Iterate over the gotten types
            foreach (var type in allTypes)
            {
                // If the result already contains a given type, increase it's needed count
                if (result.ContainsKey(type))
                {
                    result[type]++;
                }
                // Otherwise initialize the needed type with the count 1 
                else
                {
                    result.Add(type, 1);
                }
            }

            // Finally return the result
            return result;
        }

        /// <summary>
        /// A recursive method to determine the list of types contained within a single
        /// construction parameter.
        /// </summary>
        /// <param name="constructionParameter">The construction parameter.</param>
        /// <returns>The types contained within it (it might contain duplicates).</returns>
        private List<ConfigurationObjectType> ObjectTypesOf(ConstructionParameter constructionParameter)
        {
            // If we have an object parameter, we'll return a list containing only the expected type
            if (constructionParameter is ObjectConstructionParameter objectParameter)
            {
                return new List<ConfigurationObjectType> {objectParameter.ExpectedType};
            }

            // Otherwise we have a set parameter
            var setParameter = (SetConstructionParameter) constructionParameter;

            // We'll recursively find the inner types of the parameter. 
            var innerTypes = ObjectTypesOf(setParameter.TypeOfParameters);

            // And each of them repeat the needed number of times
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