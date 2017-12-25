using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        private DictionaryObjectIdResolver _idenity;

        /// <summary>
        /// The dictionary that works such that if we have two dictionary resolvers 
        /// r1, r2 with ids i1, i2, then the resolve _compositionCache[i1][i2] is 
        /// the resolver r1 o r2 (where o is the composition operator)
        /// </summary>
        private readonly Dictionary<int, Dictionary<int, DictionaryObjectIdResolver>> _compositionCache;

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
            _compositionCache = new Dictionary<int, Dictionary<int, DictionaryObjectIdResolver>>();
            Initialize(container?.LooseObjects ?? throw new ArgumentNullException(nameof(container)));
            InitializeCompositionCache();
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
                    // Cast the variations to the list
                    .Select(variation => variation.ToList())
                    // Cast them to the dictionary mapping object's indices to their ids
                    .Select
                    (
                            variation =>
                            {
                                var counter = 0;

                                var dictionary = variation.ToDictionary
                                (
                                        obj => objectsAsList[counter++].Id ?? throw GeneratorException.ObjectsIdNotSet(),
                                        obj => obj.Id ?? throw GeneratorException.ObjectsIdNotSet()
                                );

                                var result = new DictionaryObjectIdResolver(variationsCounter++, dictionary);

                                if (objectsAsList.SequenceEqual(variation))
                                {
                                    _idenity = result;
                                }

                                return result;
                            }
                    );

            // Set the resolvers (which will enumerate the enumerable)
            _resolvers.SetItems(newResolvers);
        }

        /// <summary>
        /// Initializes the composition cache dictionary.
        /// </summary>
        private void InitializeCompositionCache()
        {
            // First add initial dictionaries
            foreach (var resolver in _resolvers)
            {
                // Pull resolver id
                var resolverId = resolver.Id;

                // Add new dictionary
                _compositionCache.Add(resolverId, new Dictionary<int, DictionaryObjectIdResolver>());
            }

            // Iterate twice over resolvers
            foreach (var first in _resolvers)
            {
                // To get all possible pairs of dictionaries
                foreach (var second in _resolvers)
                {
                    // Compose dictionaries
                    var composedDictionary = first.Compose(second);

                    // Find the resolver that is equivalent to the composition result
                    var composedResoler = _resolvers.First(resolver => resolver.IsEquivalentTo(composedDictionary));

                    // Update the cache
                    _compositionCache[first.Id].Add(second.Id, composedResoler);
                }
            }
        }

        #endregion

        #region Public methods

        public DictionaryObjectIdResolver Compose(DictionaryObjectIdResolver first, DictionaryObjectIdResolver second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));

            if (second == null)
                throw new ArgumentNullException(nameof(second));

            try
            {
                return _compositionCache[first.Id][second.Id];
            }
            catch (KeyNotFoundException)
            {
                throw new GeneratorException("An attempt to get composed dictionary that were not created by this container.");
            }
        }

        public IEnumerable<DictionaryObjectIdResolver> GetNonIdenticalResolvers()
        {
            return this.Where(variation => variation != _idenity);
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