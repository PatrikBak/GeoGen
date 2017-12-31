namespace GeoGen.Generator
{
    internal class FullConfigurationToStringConverter : IFullConfigurationToStringConverter
    {
        private readonly IFullObjectToStringConverterFactory _factory;

        private readonly IConfigurationToStringProvider _provider;

        public FullConfigurationToStringConverter(IFullObjectToStringConverterFactory factory, IConfigurationToStringProvider provider)
        {
            _factory = factory;
            _provider = provider;
        }

        public string ConvertToString(ConfigurationWrapper item)
        {
            return _provider.ConvertToString(item, _factory.Get(item.ResolverToMinimalForm));
        }
    }
}