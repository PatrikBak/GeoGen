using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.LeastConfigurationFinder;

namespace GeoGen.Generator
{
    /// <summary>
    /// An abstract implementation of <see cref="IObjectToStringConverter"/> that
    /// uses custom <see cref="IObjectIdResolver"/>. It converts an object to a string
    /// using only ids of loose objects. This is useful for determining comparing
    /// configurations objects (<see cref="DefaultFullObjectToStringConverter"/>, or
    /// for determining symmetric configurations (<see cref="CustomFullObjectToStringConverter"/>
    /// and <see cref="LeastConfigurationFinder"/>). This sealed class is not thread-safe.
    /// </summary>
    internal abstract class FullObjectToStringConverterBase : IObjectToStringConverter
    {
        #region Public abstract properties

        /// <summary>
        /// Gets the unique id of this converter. 
        /// </summary>
        public abstract int Id { get; }

        #endregion

        #region Private fields

        /// <summary>
        /// The arguments list to string provider
        /// </summary>
        private readonly IArgumentsListToStringConverter _argumentsListToStringConverter;

        #endregion

        #region Protected fields

        /// <summary>
        /// The object id resolver.
        /// </summary>
        protected readonly IObjectIdResolver Resolver;

        /// <summary>
        /// The cache dictionary mapping constructed object's ids to their 
        /// string versions. 
        /// </summary>
        protected readonly Dictionary<int, string> Cache = new Dictionary<int, string>();

        #endregion
        
        #region Constructor

        /// <summary>
        /// Constructs a new full object to string provider with a given
        /// arguments list to string provider and a given object id resolver.
        /// </summary>
        /// <param name="converter>The arguments list to string provider.</param>
        /// <param name="resolver">The configuration object id resolver.</param>
        protected FullObjectToStringConverterBase(IArgumentsListToStringConverter converter, IObjectIdResolver resolver)
        {
            _argumentsListToStringConverter = converter ?? throw new ArgumentNullException(nameof(converter));
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
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

            // We let the resolver to resolve the loose objects id
            if (configurationObject is LooseConfigurationObject looseConfigurationObject)
                return $"{Resolver.ResolveId(looseConfigurationObject)}";

            // Call the abstract method to resolve the value from cache.
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
            var argumentsString = _argumentsListToStringConverter.ConvertToString(passedArgs, this);

            // Construct the beginning of the result
            var result = $"{contructedObject.Construction.Id}{argumentsString}";

            // If the object doesn't have a default index (which is 0), then we can to include it
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