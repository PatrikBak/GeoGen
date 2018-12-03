namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an <see cref="IObjectToStringConverter"/> that converts an object to a string
    /// using only ids of loose objects. This is useful for comparing configuration
    /// objects when they don't have their ids yet (<see cref="IDefaultFullObjectToStringConverter"/>,
    /// or when we need to resolve symmetric configuration by using this type of conversion
    /// (for more information, see the documentation of <see cref="IMinimalFormResolver"/> and
    /// its implementation). In that case, the objects already have their ids and we can use
    /// <see cref="IAutocacheFullObjectToStringConverter"/>.
    /// </summary>
    internal interface IFullObjectToStringConverter : IObjectToStringConverter
    {
    }
}