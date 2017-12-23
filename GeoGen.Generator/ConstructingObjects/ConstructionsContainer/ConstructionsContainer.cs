using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConstructionsContainer"/>.
    /// It has the constructor that expects initial constructions to be passed in.
    /// The <see cref="ConstructionWrapper"/>s will be then created from the them.
    /// </summary>
    internal sealed class ConstructionsContainer : IConstructionsContainer
    {
        #region Private fields

        /// <summary>
        /// The constructions list
        /// </summary>
        private readonly List<ConstructionWrapper> _constructions = new List<ConstructionWrapper>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a container and adds given initial constructions to it.
        /// </summary>
        /// <param name="initialConstructions">The initial constructions.</param>
        public ConstructionsContainer(IEnumerable<Construction> initialConstructions)
        {
            Initialize(initialConstructions);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the container with a given enumerable of constructions.
        /// The ids of the constructions must be set.
        /// </summary>
        /// <param name="constructions">The constructions enumerable.</param>
        private void Initialize(IEnumerable<Construction> constructions)
        {
            if (constructions == null)
                throw new ArgumentNullException(nameof(constructions));

            // Enumerate the constructions
            var constructionsList = constructions.ToList();

            // Make sure that all constructions have unique ids
            CheckUniqueIds(constructionsList);

            // Set new items to the container casted to the construction wrapper
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
        /// Checks if given constructions have unique ids.
        /// </summary>
        /// <param name="constructions">The constructions.</param>
        private static void CheckUniqueIds(IReadOnlyCollection<Construction> constructions)
        {
            var ids = constructions.Select(construction => construction ?? throw GeneratorException.ConstructionMostNotBeNull())
                    .Select(construction => construction.Id ?? throw GeneratorException.ConstructionIdMustBeSet())
                    .ToSet();

            if (ids.Count != constructions.Count)
            {
                throw GeneratorException.ConstructionsMustHaveUniqueId();
            }
        }

        /// <summary>
        /// Gets the type to count of objects of a given type dictionary for a given construction.
        /// </summary>
        /// <param name="construction">The construction.</param>
        /// <returns>The dictionary.</returns>
        private static Dictionary<ConfigurationObjectType, int> DetermineObjectTypesToCount(Construction construction)
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
        /// <returns>The types contained within it.</returns>
        private static List<ConfigurationObjectType> ObjectTypesOf(ConstructionParameter constructionParameter)
        {
            // If we have an object parameter, we'll return a list containing only the expected type
            if (constructionParameter is ObjectConstructionParameter objectParameter)
                return new List<ConfigurationObjectType> {objectParameter.ExpectedType};

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