using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Core.Constructions.Arguments
{
    /// <summary>
    /// Represents a set of <see cref="ConstructionArgument"/> that is passable as a <see cref="ConstructionParameter"/>.
    /// The type of passed arguments might be a <see cref="ObjectConstructionArgument"/>, or another set of arguments.
    /// It's size is not supposed to be, since it's either a <see cref="ObjectConstructionArgument"/>, or a set
    /// within a set (which doesn't make sense in our context). 
    /// </summary>
    public class SetConstructionArgument : ConstructionArgument
    {
    }
}