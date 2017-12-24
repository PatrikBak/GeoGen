namespace GeoGen.Generator
{
    internal interface IResolversComposer
    {
        IObjectIdResolver Compose(IObjectIdResolver first, IObjectIdResolver second);
    }
}
