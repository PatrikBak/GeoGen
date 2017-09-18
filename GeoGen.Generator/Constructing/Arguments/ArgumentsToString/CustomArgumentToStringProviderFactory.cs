using System;
using System.Collections.Generic;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;

namespace GeoGen.Generator.Constructing.Arguments.ArgumentsToString
{
    /// <summary>
    /// A default implementation of <see cref="ICustomArgumentToStringProviderFactory"/>
    /// </summary>
    internal class CustomArgumentToStringProviderFactory : ICustomArgumentToStringProviderFactory
    {
        #region Private fields
        
        /// <summary>
        /// The cache dictionary mapping object to string providers to their
        /// corresponding custom argument to string providers.
        /// </summary>
        private readonly Dictionary<IObjectToStringProvider, CustomArgumentToStringProvider> _cache;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomArgumentToStringProviderFactory()
        {
            _cache = new Dictionary<IObjectToStringProvider, CustomArgumentToStringProvider>();
        }

        #endregion

        #region ICustomArgumentToStringProviderFactory methods

        /// <summary>
        /// Gets the custom argument to string provider corresponding to a given
        /// object to string provider.
        /// </summary>
        /// <param name="provider">The object to string provider.</param>
        /// <returns>The custom argument to string provider.</returns>
        public CustomArgumentToStringProvider GetProvider(IObjectToStringProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            if (_cache.ContainsKey(provider))
                return _cache[provider];

            var newProvider = new CustomArgumentToStringProvider(provider);
            _cache.Add(provider, newProvider);

            return newProvider;
        }

        #endregion
    }
}