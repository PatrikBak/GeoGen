using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving
{
    /// <summary>
    /// An implementation of <see cref="ILooseConfigurationObjectIdResolver"/> that uses a dictionary
    /// mapping actual ids to resolved ids.
    /// </summary>
    internal class DictionaryBasedLooseConfigurationObjectIdResolver : ILooseConfigurationObjectIdResolver
    {
        #region Private fields

        /// <summary>
        /// The real id to the resolved id dictionary
        /// </summary>
        private readonly Dictionary<int, int> _realIdToResolvedId;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new dictionary based resolver with a given ids dictionary.
        /// </summary>
        /// <param name="realIdToResolvedId">The dictionary mapping real ids to resolved ids.</param>
        public DictionaryBasedLooseConfigurationObjectIdResolver(Dictionary<int, int> realIdToResolvedId)
        {
            _realIdToResolvedId = realIdToResolvedId ?? throw new ArgumentNullException(nameof(realIdToResolvedId));
        }

        #endregion

        #region ILooseConfigurationObjectIdResolver implementation

        /// <summary>
        /// Resolve the id of a given loose configuration object.
        /// </summary>
        /// <param name="looseConfigurationObject">The loose configuration object.</param>
        /// <returns>The id.</returns>
        public int ResolveId(LooseConfigurationObject looseConfigurationObject)
        {
            if (looseConfigurationObject == null)
                throw new ArgumentNullException(nameof(looseConfigurationObject));

            var id = looseConfigurationObject.Id ?? throw new GeneratorException("Loose configuration object without id");

            return _realIdToResolvedId[id];
        }

        #endregion
    }
}