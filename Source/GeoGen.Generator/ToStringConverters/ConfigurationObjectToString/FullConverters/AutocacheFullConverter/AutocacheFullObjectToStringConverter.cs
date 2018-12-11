using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IAutocacheFullObjectToStringConverter"/>.
    /// </summary>
    internal class AutocacheFullObjectToStringConverter : FullObjectToStringConverterBase, IAutocacheFullObjectToStringConverter
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The converter of arguments to string.</param>
        /// <param name="resolver">The object id resolver used by this converter.</param>
        public AutocacheFullObjectToStringConverter(IArgumentsToStringProvider provider, IObjectIdResolver resolver)
                : base(provider, resolver)
        {
        }

        #endregion

        #region Abstract methods from base implementation

        /// <summary>
        /// Resolves if a given object has it's string value already cached.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The cached value, if exists, otherwise an empty string.</returns>
        protected override string ResolveCachedValue(ConfigurationObject configurationObject)
        {
            // We must have an id
            var id = configurationObject.Id;

            // Then we might or might have cached this object.
            return Cache.ContainsKey(id) ? Cache[id] : string.Empty;
        }

        /// <summary>
        /// Handles the resulting string value after constructing it, before returning it.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="result">The object's string value.</param>
        protected override void HandleResult(ConfigurationObject configurationObject, string result)
        {
            // We must have an id
            var id = configurationObject.Id;

            // Then we can cache the object
            Cache.Add(id, result);
        }

        #endregion
    }
}