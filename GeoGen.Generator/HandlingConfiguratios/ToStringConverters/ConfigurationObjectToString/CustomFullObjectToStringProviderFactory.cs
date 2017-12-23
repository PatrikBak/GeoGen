using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="ICustomFullObjectToStringProviderFactory"/>.
    /// It caches given results and compares them by the id of <see cref="DictionaryObjectIdResolver"/>s.
    /// This sealed class is not thread-safe.
    /// </summary>
    internal sealed class CustomFullObjectToStringProviderFactory : ICustomFullObjectToStringProviderFactory
    {
        #region Private fields

        /// <summary>
        /// The arguments list to string provider.
        /// </summary>
        private readonly IArgumentsListToStringConverter _argumentsListToStringConverter;

        /// <summary>
        /// The dictionary mapping ids of dictionary object id resolvers
        /// to particular custom full object to string providers.
        /// </summary>
        private readonly Dictionary<int, CustomFullObjectToStringConverter> _cache;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new custom full object to string provider factory
        /// that uses a given arguments list to string provider for creating
        /// instances of <see cref="CustomFullObjectToStringConverter"/>.
        /// </summary>
        /// <param name="converter>The arguments list to string provider.</param>
        public CustomFullObjectToStringProviderFactory(IArgumentsListToStringConverter converter)
        {
            _argumentsListToStringConverter = converter ?? throw new ArgumentNullException(nameof(converter));
            _cache = new Dictionary<int, CustomFullObjectToStringConverter>();
        }

        #endregion

        #region IObjectToStringProviderFactory implementation

        /// <summary>
        /// Creates an instance of <see cref="CustomFullObjectToStringConverter"/>
        /// that uses a given dictionary object id resolver as its id resolver.
        /// </summary>
        /// <param name="resolver">The dictionary object id resolver.</param>
        /// <returns>The custom full object to string provider.</returns>
        public CustomFullObjectToStringConverter GetCustomProvider(DictionaryObjectIdResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var id = resolver.Id;

            if (_cache.ContainsKey(id))
                return _cache[id];

            var newResolver = new CustomFullObjectToStringConverter(_argumentsListToStringConverter, resolver);
            _cache.Add(id, newResolver);

            return newResolver;
        }

        #endregion
    }
}