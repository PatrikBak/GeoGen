using System;
using System.Collections.Concurrent;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An abstract implementation of <see cref="IObjectToStringProvider"/> that
    /// uses custom <see cref="IObjectIdResolver"/>. It converts an object to a string
    /// using only ids of loose objects. This is useful for determining symetric configurations.
    /// It has two implementations, <see cref="DefaultFullObjectToStringProvider"/>
    /// and <see cref="CustomFullObjectToStringProvider"/>.
    /// </summary>
    internal abstract class FullObjectToStringProviderBase : ObjectToStringProviderBase
    {
        #region Private fields

        /// <summary>
        /// The arguments list to string provider
        /// </summary>
        private readonly IArgumentsListToStringProvider _argumentsListToStringProvider;

        #endregion

        #region Protected fields

        /// <summary>
        /// The cache dictionary mapping constructed object's ids to their 
        /// string versions. 
        /// </summary>
        protected readonly ConcurrentDictionary<int, string> Cache = new ConcurrentDictionary<int, string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new complex configuration object to string provider with a given
        /// arguments list to string provider and a given object id resolver.
        /// </summary>
        /// <param name="provider">The arguments list to string provider.</param>
        /// <param name="resolver">The configuration object id resolver.</param>
        protected FullObjectToStringProviderBase(IArgumentsListToStringProvider provider, IObjectIdResolver resolver)
            : base(resolver)
        {
            _argumentsListToStringProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region ObjectToStringProviderBase methods

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the list.</returns>
        public override string ConvertToString(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            // We let the resolvedrto resolve the loose objects ids
            if (configurationObject is LooseConfigurationObject looseConfigurationObject)
                return $"{Resolver.ResolveId(looseConfigurationObject)}";

            // Call the abstract method to resolve the value from cache.
            var cachedString = ResolveCachedValue(configurationObject);

            // If there is any non-empty cached value, return it immediately
            if (cachedString != string.Empty)
                return cachedString;

            // The object must be a constructed configuration object
            var contructedObject = configurationObject as ConstructedConfigurationObject ?? throw new GeneratorException("Unhandled case");

            // We find arguments string. This might cause recursive calls of this very function,
            // because we're passing this object as an object to string provider. 
            var argumentsString = _argumentsListToStringProvider.ConvertToString(contructedObject.PassedArguments, this);

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