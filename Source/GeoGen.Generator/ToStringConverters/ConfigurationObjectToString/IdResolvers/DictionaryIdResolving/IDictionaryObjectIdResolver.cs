namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an <see cref="IObjectIdResolver"/> that uses a dictionary
    /// mapping actual ids to the ids to be resolved.
    /// </summary>
    internal interface IDictionaryObjectIdResolver : IObjectIdResolver
    {
    }
}