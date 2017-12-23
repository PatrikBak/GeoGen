using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IDictionaryObjectIdResolversContainer"/>.
    /// </summary>
    internal sealed class DictionaryObjectIdResolversContainer : IDictionaryObjectIdResolversContainer
    {
        #region Private fields

        /// <summary>
        /// The resolvers list.
        /// </summary>
        private readonly List<DictionaryObjectIdResolver> _resolvers = new List<DictionaryObjectIdResolver>();

        /// <summary>
        /// The variations provider.
        /// </summary>
        private readonly IVariationsProvider _variationsProvider;

        #endregion

        #region Constructor

        // TODO: FIX DOC
        /// <summary>
        /// Constructs a dictionary object id resolvers container that creates
        /// all possible permutations of ids of given configuration objects
        /// using a given variations provider.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <param name="variationsProvider">The variations provider.</param>
        public DictionaryObjectIdResolversContainer(IConfigurationObjectsContainer container, IVariationsProvider variationsProvider)
        {
            _variationsProvider = variationsProvider ?? throw new ArgumentNullException(nameof(variationsProvider));
            Initialize(container.LooseObjects);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the container with a non-empty list of configuration objects.
        /// </summary>
        /// <param name="objects">The configuration objects list.</param>
        private void Initialize(IEnumerable<ConfigurationObject> objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            // Cast objects to list
            var objectsAsList = objects.ToList();

            if (objectsAsList.Empty())
                throw new ArgumentException("Objects list can't be empty.");

            // We need to start from 1, because 0 is the id of the default resolver.
            var variationsCounter = 1;

            // Create resolvers
            var newResolvers = _variationsProvider
                    // Get variations
                    .GetVariations(objectsAsList, objectsAsList.Count)
                    // Cast them to the dictionary mapping object's indices to their ids
                    .Select
                    (
                        variation =>
                        {
                            var counter = 0;

                            var dictionary = variation.ToDictionary
                            (
                                obj => objectsAsList[counter++].Id ?? throw new GeneratorException("Id must be set"),
                                obj => obj.Id ?? throw new GeneratorException("Id must be set")
                            );

                            return new DictionaryObjectIdResolver(variationsCounter++, dictionary);
                        }
                    );

            // Set the resolvers (which will enumerate the enumerable)
            _resolvers.SetItems(newResolvers);
        }

        #endregion

        #region IEnumerable methods

        public IEnumerator<DictionaryObjectIdResolver> GetEnumerator()
        {
            return _resolvers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}