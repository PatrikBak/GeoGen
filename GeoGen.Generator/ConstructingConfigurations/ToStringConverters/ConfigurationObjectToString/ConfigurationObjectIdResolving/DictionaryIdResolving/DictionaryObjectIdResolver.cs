using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IDictionaryObjectIdResolver"/>.
    /// </summary>
    internal class DictionaryObjectIdResolver : IDictionaryObjectIdResolver
    {
        #region Private fields

        /// <summary>
        /// The real ids to the resolved ids dictionary
        /// </summary>
        private readonly IReadOnlyDictionary<int, int> _realIdsToResolvedIds;

        #endregion

        #region IObjectIdResolver properties

        /// <summary>
        /// Gets the id of the resolver.
        /// </summary>
        public int Id { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="id">The id of the resolver. The id must be distinct </param>
        /// <param name="realIdsToResolvedIds">The dictionary mapping real ids to resolved ids.</param>
        public DictionaryObjectIdResolver(int id, IReadOnlyDictionary<int, int> realIdsToResolvedIds)
        {
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
            var id = configurationObject.Id ?? throw GeneratorException.ConfigurationIdNotSet();

            return _realIdsToResolvedIds[id];
        }

        #endregion
    }
}