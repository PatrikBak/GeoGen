using System;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IFullObjectToStringConvertersFactory"/>.
    /// </summary>
    internal class FullObjectToStringConvertersFactory : IFullObjectToStringConvertersFactory
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping ids of <see cref="IObjectIdResolver"/> to the
        /// corresponding full object to string converters.
        /// </summary>
        private readonly Dictionary<int, IFullObjectToStringConverter> _cache;

        /// <summary>
        /// The factory for creating <see cref="IAutocacheFullObjectToStringConverter"/>s.
        /// </summary>
        private readonly IAutocacheFullObjectToStringConverterFactory _factory; 

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="converter">The default converter.</param>
        /// <param name="factory">The factory for creating auto-cache converters.</param>
        public FullObjectToStringConvertersFactory(IDefaultFullObjectToStringConverter converter, IAutocacheFullObjectToStringConverterFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            var resolverId = converter?.Resolver.Id ?? throw new ArgumentNullException(nameof(converter));
            _cache = new Dictionary<int, IFullObjectToStringConverter> {{resolverId, converter}};
        }

        #endregion

        #region IFullObjectToStringConvertersFactory implementation

        /// <summary>
        /// Gets a full object to string converted associated with a given
        /// object id resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The converter.</returns>
        public IFullObjectToStringConverter Get(IObjectIdResolver resolver)
        {
            // Take the id
            var id = resolver.Id;

            // If we have it cached, return it
            if (_cache.ContainsKey(id))
                return _cache[id];

            // Otherwise we let the factory create the converter
            var converter = _factory.Create(resolver);

            // Cache it
            _cache.Add(id, converter);

            // And return the converter
            return converter;
        }

        #endregion
    }
}