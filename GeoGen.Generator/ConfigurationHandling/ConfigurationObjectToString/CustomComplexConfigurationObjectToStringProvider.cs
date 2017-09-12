using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.LooseObjectIdResolving;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// An implementation of <see cref="ComplexConfigurationObjectToStringProviderBase"/> that
    /// uses custom <see cref="ILooseConfigurationObjectIdResolver"/> (distint from 
    /// <see cref="DefaultLooseConfigurationObjectIdResolver"/>.) This class is meant to be 
    /// used during the symetric configurations detection, together with 
    /// <see cref="DictionaryBasedLooseConfigurationObjectIdResolver"/>. It expectes 
    /// that all objects have their ids already set. It automatically caches the evaluated 
    /// results (unlike <see cref="DefaultComplexConfigurationObjectToStringProvider"/>), 
    /// since it already has the ids available.
    /// </summary>
    internal class CustomComplexConfigurationObjectToStringProvider : ComplexConfigurationObjectToStringProviderBase
    {
        #region Constructor

        /// Constructs a new custom complex configuration object to string provider 
        /// with a given arguments to string provider and a given loose configuration 
        /// object id resolver.
        /// <param name="argumentsToStringProvider">The arguments to string provider.</param>
        /// <param name="looseConfigurationObjectIdResolver">The loose configuration object id resolver.</param>
        public CustomComplexConfigurationObjectToStringProvider(IArgumentsToStringProvider argumentsToStringProvider,
            ILooseConfigurationObjectIdResolver looseConfigurationObjectIdResolver)
            : base(argumentsToStringProvider, looseConfigurationObjectIdResolver)
        {
        }

        #endregion

        #region Abstract methods from base implementation

        /// <summary>
        /// Resolve if a given object has it's to string value already cached.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The cached value, if exists, otherwise an empty string.</returns>
        protected override string ResolveCachedValue(ConfigurationObject configurationObject)
        {
            // We must have an id
            var id = configurationObject.Id ?? throw new GeneratorException("Value must be set");

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
            // We want to do the manual caching here.
            Cache.TryAdd(configurationObject.Id ?? throw new GeneratorException(), result);
        }

        #endregion
    }
}