using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IObjectIdResolversContainer"/>.
    /// </summary>
    internal class ObjectIdResolversContainer : IObjectIdResolversContainer
    {
        #region Private fields

        /// <summary>
        /// The variations provider for generating all possible permutations.
        /// (a permutation is a special case of a variation).
        /// </summary>
        private readonly IVariationsProvider _variationsProvider;

        /// <summary>
        /// The default (identical) id resolver. Others will be generated.
        /// </summary>
        private readonly IDefaultObjectIdResolver _resolver;

        /// <summary>
        /// The list of all resolvers that this container contain.
        /// </summary>
        private readonly List<IObjectIdResolver> _resolvers;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="holder">The holder of identified loose objects.</param>
        /// <param name="provider">The variations (permutations) generator.</param>
        /// <param name="resolver">The default (identical) id resolver.</param>
        public ObjectIdResolversContainer(ILooseObjectsHolder holder, IVariationsProvider provider, IDefaultObjectIdResolver resolver)
        {
            _variationsProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _resolvers = new List<IObjectIdResolver>();
            Initialize(holder?.LooseObjects ?? throw new ArgumentNullException(nameof(holder)));
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the container with a non-empty list of loose configuration objects.
        /// </summary>
        /// <param name="objects">The loose objects.</param>
        private void Initialize(IEnumerable<LooseConfigurationObject> objects)
        {
            // Cast objects to ids list
            var ids = objects
                    .Select(o => o.Id ?? throw new GeneratorException("Configuration objects id must be set."))
                    .ToList();

            // Start from the id of the default resolver + 1 so we don't get a conflict
            var variationsCounter = _resolver.Id + 1;

            // Create resolvers enumerable
            var newResolvers = _variationsProvider
                    // Get permutations
                    .GetVariations(ids, ids.Count)
                    // Cast them to the corresponding resolver
                    .Select(permutation =>
                    {
                        // Cast the permutation to list
                        var permutationAsList = permutation.ToList();

                        // Initialize the counter
                        var counter = 0;

                        // Create the dictionary mapping original ids to resolved ids
                        var dictionary = permutationAsList.ToDictionary(id => ids[counter++], id => id);

                        // Find out if the permutation is identical
                        var isIdentical = permutationAsList.SequenceEqual(ids);

                        // If yes, return the default resolver
                        if (isIdentical)
                            return (IObjectIdResolver) _resolver;

                        // Otherwise create the dictionary resolver
                        return new DictionaryObjectIdResolver(variationsCounter++, dictionary);
                    });

            // Set the resolvers (which will enumerate the enumerable)
            _resolvers.SetItems(newResolvers);
        }

        #endregion

        #region IEnumerable methods

        public IEnumerator<IObjectIdResolver> GetEnumerator()
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