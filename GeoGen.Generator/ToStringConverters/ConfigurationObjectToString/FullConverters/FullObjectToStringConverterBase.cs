using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default template implementation of <see cref="IFullConfigurationToStringConverter"/>.
    /// </summary>
    internal abstract class FullObjectToStringConverterBase : IFullObjectToStringConverter
    {
        #region IFullObjectToStringConverter properties

        /// <summary>
        /// Gets the object id resolver associated with this converter.
        /// </summary>
        public IObjectIdResolver Resolver { get; }

        #endregion

        #region Private fields

        /// <summary>
        /// The converter of arguments to string.
        /// </summary>
        private readonly IArgumentsToStringProvider _argumentsToStringProvider;

        #endregion

        #region Protected fields

        /// <summary>
        /// The cache dictionary mapping object's ids to their string versions. 
        /// </summary>
        protected readonly Dictionary<int, string> Cache = new Dictionary<int, string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The converter of arguments to string.</param>
        /// <param name="resolver">The object id resolver used by this converter.</param>
        protected FullObjectToStringConverterBase(IArgumentsToStringProvider provider, IObjectIdResolver resolver)
        {
            _argumentsToStringProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        #endregion

        #region IFullObjectToStringConverter methods

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            // We let the resolver to resolve the loose objects id
            if (configurationObject is LooseConfigurationObject looseConfigurationObject)
                return $"{Resolver.ResolveId(looseConfigurationObject)}";

            // Call the abstract method to resolve the value from the cache.
            var cachedString = ResolveCachedValue(configurationObject);

            // If there is any non-empty cached value, return it immediately
            if (cachedString != string.Empty)
                return cachedString;

            // The object must be a constructed configuration object
            var contructedObject = (ConstructedConfigurationObject) configurationObject;

            // Pull the passed arguments
            var passedArgs = contructedObject.PassedArguments;

            // We find arguments string. This might cause recursive calls of this function,
            // because we're passing this object as an object to string provider. 
            var argumentsString = _argumentsToStringProvider.ConvertToString(passedArgs, this);

            // Construct the beginning of the result
            var result = $"{contructedObject.Construction.Id}{argumentsString}";

            // If the object doesn't have a default index (which is 0), then we have to include it
            if (contructedObject.Index != 0)
                result += $"[{contructedObject.Index}]";

            // Let the abstract method to do something with the result before returning it
            HandleResult(configurationObject, result);

            // And finally return the result
            return result;
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Resolves if a given object has it's string value already cached.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The cached value, if exists, otherwise an empty string.</returns>
        protected abstract string ResolveCachedValue(ConfigurationObject configurationObject);

        /// <summary>
        /// Handles the resulting string value after constructing it, before returning it.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="result">The object's string value.</param>
        protected abstract void HandleResult(ConfigurationObject configurationObject, string result);

        #endregion
    }
}