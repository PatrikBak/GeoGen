using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// An implementation of <see cref="IObjectIdResolver"/> that uses a dictionary
    /// mapping actual ids to the ids to be resolved.
    /// </summary>
    internal sealed class DictionaryObjectIdResolver : IObjectIdResolver
    {
        #region Private fields

        /// <summary>
        /// The real ids to the resolved ids dictionary
        /// </summary>
        private readonly Dictionary<int, int> _realIdsToResolvedIds;

        #endregion

        #region IObjectIdResolver properties

        /// <summary>
        /// Gets the id of the resolver.
        /// </summary>
        public int Id { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new dictionary object id with a given id and 
        /// a given ids dictionary. The id can't be the same as the id 
        /// of the <see cref="DefaultObjectIdResolver"/>.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="realIdsToResolvedIds">The dictionary mapping real ids to resolved ids.</param>
        public DictionaryObjectIdResolver(int id, Dictionary<int, int> realIdsToResolvedIds)
        {
            if (id == DefaultObjectIdResolver.DefaultId)
                throw new ArgumentException("The id can't be the same as the default object id resolver's id");

            Id = id;
            _realIdsToResolvedIds = realIdsToResolvedIds ?? throw new ArgumentNullException(nameof(realIdsToResolvedIds));
        }

        #endregion

        #region IObjectIdResolver methods

        /// <summary>
        /// Resolves the id of a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The id.</returns>
        public int ResolveId(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            var id = configurationObject.Id ?? throw new GeneratorException("Id must be set.");

            return _realIdsToResolvedIds[id];
        }

        #endregion

        #region Public methods

        public Dictionary<int, int> Compose(DictionaryObjectIdResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            if (resolver._realIdsToResolvedIds.Count != _realIdsToResolvedIds.Count)
                throw new ArgumentException("Cannot compose two dictionary resolvers with distinct number of elements.");

            var result = new Dictionary<int, int>();

            foreach (var pair in _realIdsToResolvedIds)
            {
                var key = pair.Key;

                var value = pair.Value;

                if (!resolver._realIdsToResolvedIds.ContainsKey(value))
                    throw new ArgumentException("Values of the first dictionary resolver doesn't match the key of the second one.");

                result.Add(key, resolver._realIdsToResolvedIds[value]);
            }

            return result;
        }

        /// <summary>
        /// Checks if the functionality of the dictionary is equivalently
        /// shown in the provided dictionary.
        /// </summary>
        /// <param name="idsDictionary">The ids dictionary.</param>
        public bool IsEquivalentTo(Dictionary<int, int> idsDictionary)
        {
            if (idsDictionary == null)
                throw new ArgumentNullException(nameof(idsDictionary));

            if (_realIdsToResolvedIds.Count != idsDictionary.Count)
                return false;

            foreach (var pair in idsDictionary)
            {
                var key = pair.Key;

                if (!_realIdsToResolvedIds.ContainsKey(key))
                    return false;

                var internalValue = _realIdsToResolvedIds[key];

                if (internalValue != pair.Value)
                    return false;
            }

            return true;
        }

        #endregion
    }
}