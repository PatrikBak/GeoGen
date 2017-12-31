namespace GeoGen.Generator
{
    internal class FullObjectToStringConverterFactory : IFullObjectToStringConverterFactory
    {
        private readonly IDefaultFullObjectToStringConverter _defaultConverter;

        private readonly ICustomFullObjectToStringConverterFactory _customConverterFactory;

        public FullObjectToStringConverterFactory(IDefaultFullObjectToStringConverter defaultConverter, ICustomFullObjectToStringConverterFactory customConverterFactory)
        {
            _defaultConverter = defaultConverter;
            _customConverterFactory = customConverterFactory;
        }

        public IObjectToStringConverter Get(IObjectIdResolver resolver)
        {
            if (resolver is IDefaultObjectIdResolver)
            {
                return _defaultConverter;
            }

            var dictionaryResolver = (DictionaryObjectIdResolver) resolver;

            return _customConverterFactory.GetCustomProvider(dictionaryResolver);
        }
    }
}