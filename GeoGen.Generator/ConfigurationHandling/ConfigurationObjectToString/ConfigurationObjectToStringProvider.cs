using System;
using GeoGen.Core.Configurations;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// A default implementation of <see cref="IConfgurationObjectToStringProvider"/>.
    /// </summary>
    internal class ConfigurationObjectToStringProvider : IConfgurationObjectToStringProvider
    {
        #region Private fields

        /// <summary>
        /// The default separator. Must be different than the separator 
        /// of the string provider.
        /// </summary>
        private const string Separator = "-";

        /// <summary>
        /// The arguments to string provider
        /// </summary>
        private readonly IArgumentsToStringProvider _argumentsToStringProvider;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Constructs a new configuration object to string provider with a given
        /// arguments to string provider.
        /// </summary>
        /// <param name="argumentsToStringProvider">The arguments to string provider.</param>
        public ConfigurationObjectToStringProvider(IArgumentsToStringProvider argumentsToStringProvider)
        {
            _argumentsToStringProvider = argumentsToStringProvider;
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

            if (configurationObject is LooseConfigurationObject)
                return $"{configurationObject.Id}";

            var contructedObject = configurationObject as ConstructedConfigurationObject ?? throw new GeneratorException("Unhandled case");
            var argumentsString = _argumentsToStringProvider.ConvertToString(contructedObject.PassedArguments);

            return $"{contructedObject.Construction.Id}{Separator}{contructedObject.Index}{Separator}{argumentsString}";
        } 
        #endregion
    }
}