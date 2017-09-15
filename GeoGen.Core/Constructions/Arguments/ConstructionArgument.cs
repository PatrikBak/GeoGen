namespace GeoGen.Core.Constructions.Arguments
{
    /// <summary>
    /// Represents an argument that is passed to a <see cref="Construction"/>. It holds a value, not a definition.
    /// </summary>
    public abstract class ConstructionArgument
    {
        public int? Id { get; set; }
    }
}