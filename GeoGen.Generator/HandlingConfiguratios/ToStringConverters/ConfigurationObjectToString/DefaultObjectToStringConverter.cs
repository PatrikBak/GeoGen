using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IObjectToStringConverter"/>
    /// that uses a <see cref="DefaultObjectIdResolver"/>.
    /// </summary>
    internal sealed class DefaultObjectToStringConverter : IDefaultObjectToStringConverter
    {
        #region Public constants

        /// <summary>
        /// The default id of the converter.
        /// </summary>
        public const int DefaultId = 0;

        #endregion

        #region IObjectToStringConverter properties

        /// <summary>
        /// Gets the object to string resolver that is used by this provider.
        /// </summary>
        public IObjectIdResolver Resolver { get; }

        /// <summary>
        /// Gets the unique id of this converter. 
        /// </summary>
        public int Id { get; } = DefaultId;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new default object to string provider with a given
        /// default object id resolver.
        /// </summary>
        /// <param name="defaultResolver">The default object id resolver.</param>
        public DefaultObjectToStringConverter(IDefaultObjectIdResolver defaultResolver)
        {
            Resolver = defaultResolver ?? throw new ArgumentNullException(nameof(defaultResolver));
        }

        #endregion

        #region ObjectToStringProviderBase methods

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            if (!configurationObject.Id.HasValue)
                throw new GeneratorException("The configuration object doesn't have an id.");

            return Resolver.ResolveId(configurationObject).ToString();
        }

        #endregion
    }
}