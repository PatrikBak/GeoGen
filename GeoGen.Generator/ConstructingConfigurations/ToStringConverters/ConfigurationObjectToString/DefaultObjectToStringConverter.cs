using System;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IObjectToStringConverter"/>
    /// that uses a <see cref="IDefaultObjectIdResolver"/>.
    /// </summary>
    internal class DefaultObjectToStringConverter : IDefaultObjectToStringConverter
    {
        #region IDefaultObjectToStringConverter properties

        /// <summary>
        /// Gets the object id resolver associated with this converter.
        /// </summary>
        public IObjectIdResolver Resolver { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="defaultResolver">The default id resolver.</param>
        public DefaultObjectToStringConverter(IDefaultObjectIdResolver defaultResolver)
        {
            Resolver = defaultResolver ?? throw new ArgumentNullException(nameof(defaultResolver));
        }

        #endregion

        #region IDefaultObjectToStringConverter methods

        /// <summary>
        /// Converts a given configuration object to string.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            return Resolver.ResolveId(configurationObject).ToString();
        }

        #endregion
    }
}