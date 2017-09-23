using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.VariationsProviding;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving
{
    /// <summary>
    /// A default implementation of <see cref="IDictionaryObjectIdResolversContainer"/>.
    /// This class is not thread-safe.
    /// </summary>
    internal class DictionaryObjectIdResolversContainer : IDictionaryObjectIdResolversContainer
    {
        #region Private fields

        /// <summary>
        /// The resolvers list.
        /// </summary>
        private readonly List<DictionaryObjectIdResolver> _resolvers = new List<DictionaryObjectIdResolver>();

        /// <summary>
        /// The variations provider.
        /// </summary>
        private readonly IVariationsProvider<LooseConfigurationObject> _variationsProvider;

        /// <summary>
        /// Indicates if the container has been initialized.
        /// </summary>
        private bool _initialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new dictionary object id resolvers container
        /// that uses a given variations provider of loose configuration
        /// objects.
        /// </summary>
        /// <param name="variationsProvider">The variations provider.</param>
        public DictionaryObjectIdResolversContainer(IVariationsProvider<LooseConfigurationObject> variationsProvider)
        {
            _variationsProvider = variationsProvider ?? throw new ArgumentNullException(nameof(variationsProvider));
        }

        #endregion

        #region IDictionaryObjectIdResolverContainer methods

        /// <summary>
        /// Initializes the container with a non-empty list of loose
        /// configuration objects.
        /// </summary>
        /// <param name="looseConfigurationObjects">The loose configuration objects list.</param>
        public void Initialize(IReadOnlyList<LooseConfigurationObject> looseConfigurationObjects)
        {
            try
            {
                DoInitialization(looseConfigurationObjects);
                _initialized = true;
            }
            catch (Exception)
            {
                _initialized = false;
                throw;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Executes the initialization process.
        /// </summary>
        /// <param name="objects">The loose configurations objects.</param>
        private void DoInitialization(IReadOnlyList<LooseConfigurationObject> objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            if (objects.Empty())
                throw new ArgumentException("Loose objects list can't be empty.");

            // We need to start from 1, because 0 is the id of the default resolver.
            var variationsCounter = 1;

            // Create resolvers
            var newResolvers = _variationsProvider
                    // Get variations
                    .GetVariations(objects, objects.Count)
                    // Cast them to the dictionary mapping object's indices to their ids
                    .Select
                    (
                        variation =>
                        {
                            var counter = 0;

                            var dictionary = variation.ToDictionary
                            (
                                obj => objects[counter++].Id ?? throw new GeneratorException("Id must be set"),
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
            if (!_initialized)
                throw new GeneratorException("The container hasn't been initialized.");

            return _resolvers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}