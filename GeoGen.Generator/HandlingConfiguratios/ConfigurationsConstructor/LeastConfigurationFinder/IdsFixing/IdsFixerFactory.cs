using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IIdsFixerFactory"/>.
    /// It creates <see cref="IdsFixerFactory"/> objects and caches them in
    /// a dictionary. The keys of the dictionary are ids of 
    /// <see cref="DictionaryObjectIdResolver"/>. This sealed class is not thread-safe.
    /// </summary>
    internal sealed class IdsFixerFactory : IIdsFixerFactory
    {
        #region Private fields

        /// <summary>
        /// The configuration objects container.
        /// </summary>
        private readonly IConfigurationObjectsContainer _objectsContainer;

        /// <summary>
        /// The cache mapping dictionary object id resolvers' ids 
        /// to ids fixers associated with them.
        /// </summary>
        private readonly Dictionary<int, IIdsFixer> _cache;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new ids fixer factory that uses a given
        /// configuration objects container for creating <see cref="IdsFixer"/>s.
        /// </summary>
        /// <param name="objectsContainer">The configuration objects container.</param>
        public IdsFixerFactory(IConfigurationObjectsContainer objectsContainer)
        {
            _objectsContainer = objectsContainer ?? throw new ArgumentNullException(nameof(objectsContainer));
            _cache = new Dictionary<int, IIdsFixer>();
        }

        #endregion

        #region IIdsFixerFactory methods

        /// <summary>
        /// Creates an ids fixer corresponding to a given
        /// dictionary object id resolver.
        /// </summary>
        /// <param name="resolver">The dictionary object id resolver.</param>
        /// <returns>The ids fixer.</returns>
        public IIdsFixer CreateFixer(DictionaryObjectIdResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var id = resolver.Id;

            if (_cache.ContainsKey(id))
                return _cache[id];

            var newFixer = new IdsFixer(_objectsContainer, resolver);
            _cache.Add(id, newFixer);

            return newFixer;
        }

        #endregion
    }
}