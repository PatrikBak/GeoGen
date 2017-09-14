using System;
using System.Collections.Concurrent;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An abstract implementation of <see cref="IConfigurationObjectToStringProvider"/> that
    /// uses custom <see cref="IObjectIdResolver"/>. It converts an object to a string
    /// using only ids of loose objects. This is useful for determining symetric configurations.
    /// It has two implementations, <see cref="DefaultComplexConfigurationObjectToStringProvider"/>
    /// and <see cref="CustomComplexConfigurationObjectToStringProvider"/>.
    /// </summary>
    internal abstract class ComplexConfigurationObjectToStringProviderBase : IConfigurationObjectToStringProvider
    {
        #region Private fields

        /// <summary>
        /// The arguments to string provider
        /// </summary>
        private readonly IArgumentsToStringProvider _argumentsToStringProvider;

        /// <summary>
        /// The object id resolver.
        /// </summary>
        private readonly IObjectIdResolver _objectIdResolver;

        #endregion

        #region Protected fields

        /// <summary>
        /// The cache dictionary mapping constructed configuration ids to their 
        /// string versions. 
        /// </summary>
        protected readonly ConcurrentDictionary<int, string> Cache = new ConcurrentDictionary<int, string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new complex configuration object to string provider with a given
        /// arguments to string provider and a given loose object id resolver.
        /// </summary>
        /// <param name="argumentsToStringProvider">The arguments to string provider.</param>
        /// <param name="objectIdResolver">The configuration object id resolver.</param>
        protected ComplexConfigurationObjectToStringProviderBase(IArgumentsToStringProvider argumentsToStringProvider,
            IObjectIdResolver objectIdResolver)
        {
            _argumentsToStringProvider = argumentsToStringProvider ?? throw new ArgumentNullException(nameof(argumentsToStringProvider));
            _objectIdResolver = objectIdResolver ?? throw new ArgumentNullException(nameof(argumentsToStringProvider));
        }

        #endregion

        #region IConfigurationObjectToStringProvider implementation

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the list.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            // We let the resolved to resolve the loose objects ids
            if (configurationObject is LooseConfigurationObject looseConfigurationObject)
                return $"{_objectIdResolver.ResolveId(looseConfigurationObject)}";

            // Call the abstract method to resolve the value from cache.
            var cachedString = ResolveCachedValue(configurationObject);

            // If there is any non-empty cached value, return it immediately
            if (cachedString != string.Empty)
                return cachedString;

            // The object must be a constructed configuration object
            var contructedObject = configurationObject as ConstructedConfigurationObject ?? throw new GeneratorException("Unhandled case");

            // We find arguments string. This might cause recursive calls of this very function,
            // because we're passing this object as an object to string provider. 
            var argumentsString = _argumentsToStringProvider.ConvertToString(contructedObject.PassedArguments, this);

            // Construct the result
            var result = $"{contructedObject.Construction.Id}{argumentsString}[{contructedObject.Index}]";

            // Let the abstract method to something with it before returning it
            HandleResult(configurationObject, result);

            // And finally return the result
            return result;
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Resolve if a given object has it's to string value already cached.
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