using System;
using GeoGen.Core.Configurations;

namespace GeoGen.Core.Constructions.Arguments
{
    /// <summary>
    /// Represents an argument that is passed to a <see cref="Construction"/>. 
    /// It holds a value, not a definition.
    /// </summary>
    public abstract class ConstructionArgument
    {
        public abstract void Visit(Action<ConstructionArgument> actionForInternalObjects);
    }
}