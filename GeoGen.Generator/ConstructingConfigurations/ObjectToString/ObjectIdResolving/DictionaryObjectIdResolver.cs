using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving
{
    /// <summary>
    /// An implementation of <see cref="IObjectIdResolver"/> that uses a dictionary
    /// mapping actual ids to the ids to be resolved.
    /// </summary>
    internal class DictionaryObjectIdResolver : IObjectIdResolver
    {
        #region Private fields

        /// <summary>
        /// The real id to the resolved id dictionary
        /// </summary>
        private readonly Dictionary<int, int> _realIdToResolvedId;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new dictionary object id resolver with 
        /// a given ids dictionary and a given id. The id cannot be 
        /// the same as the id of the <see cref="DefaultObjectIdResolver"/>.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="realIdToResolvedId">The dictionary mapping real ids to resolved ids.</param>
        public DictionaryObjectIdResolver(int id, Dictionary<int, int> realIdToResolvedId)
        {
            if (id == DefaultObjectIdResolver.DefaultId)
                throw new ArgumentException("The id cannot be the same as the default object id resolver's id");

            Id = id;
            _realIdToResolvedId = realIdToResolvedId ?? throw new ArgumentNullException(nameof(realIdToResolvedId));
        }

        #endregion

        #region IObjectIdResolver properties

        /// <summary>
        /// The id of the resolver.
        /// </summary>
        public int Id { get; }

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

            return _realIdToResolvedId[id];
        }

        #endregion
    }
}