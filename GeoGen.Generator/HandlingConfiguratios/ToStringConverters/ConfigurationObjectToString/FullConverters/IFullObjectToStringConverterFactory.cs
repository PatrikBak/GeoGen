namespace GeoGen.Generator
{
    internal interface IFullObjectToStringConverterFactory
    {
        IObjectToStringConverter Get(IObjectIdResolver resolver);
    }
}
