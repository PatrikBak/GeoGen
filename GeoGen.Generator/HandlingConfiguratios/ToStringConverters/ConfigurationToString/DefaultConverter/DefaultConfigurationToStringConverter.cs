using System;

namespace GeoGen.Generator
{
    internal class DefaultConfigurationToStringConverter : IDefaultConfigurationToStringConverter
    {
        private readonly IConfigurationToStringProvider _configurationToString;

        private readonly IDefaultFullObjectToStringConverter _objectToString;

        public DefaultConfigurationToStringConverter
        (
                IConfigurationToStringProvider configurationToString,
                IDefaultFullObjectToStringConverter objectToString
        )
        {
            _configurationToString = configurationToString;
            _objectToString = objectToString;
        }

        public string ConvertToString(ConfigurationWrapper item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return _configurationToString.ConvertToString(item, _objectToString);
        }
    }
}