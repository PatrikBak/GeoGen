using System;
using System.Collections.Generic;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ObjectsContainer;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.IdsFixing
{
    /// <summary>
    /// A default implementation of <see cref="IIdsFixerFactory"/>.
    /// </summary>
    internal class IdsFixerFactory : IIdsFixerFactory
    {
        #region Private fields

        /// <summary>
        /// The configuration objects container.
        /// </summary>
        private readonly IConfigurationObjectsContainer _configurationObjectsContainer;

        /// <summary>
        /// The argument container;
        /// </summary>
        private readonly IArgumentContainer _argumentContainer;

        /// <summary>
        /// The cache mapping dictionary object id resolvers' ids 
        /// to ids fixers associated with them
        /// </summary>
        private readonly Dictionary<int, IIdsFixer> _cache;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new ids fixer factory that is using a given
        /// configuration objects container and a given argument
        /// container.
        /// </summary>
        /// <param name="objectsContainer">The configuration objects container.</param>
        /// <param name="argumentContainer">The argument container</param>
        public IdsFixerFactory(IConfigurationObjectsContainer objectsContainer, IArgumentContainer argumentContainer)
        {
            _argumentContainer = argumentContainer ?? throw new ArgumentNullException(nameof(argumentContainer));
            _configurationObjectsContainer = objectsContainer ?? throw new ArgumentNullException(nameof(objectsContainer));
            _cache = new Dictionary<int, IIdsFixer>();
        }

        #endregion

        #region IIdsFixerFactory methods

        /// <summary>
        /// Creates an ids fixer corresponding to a given
        /// dictionary object id resolver.
        /// </summary>
        /// <param name="resolver">The dictionry object id resolver.</param>
        /// <returns>The ids fixer.</returns>
        public IIdsFixer CreateFixer(DictionaryObjectIdResolver resolver)
        {
            var id = resolver.Id;

            if (_cache.ContainsKey(id))
                return _cache[id];

            var newFixer = new IdsFixer(_argumentContainer,_configurationObjectsContainer, resolver);
            _cache.Add(id, newFixer);

            return newFixer;
        }

        #endregion
    }
}