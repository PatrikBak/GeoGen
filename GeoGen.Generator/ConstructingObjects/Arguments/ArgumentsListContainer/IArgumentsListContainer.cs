using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a container that is able to recognize the same arguments 
    /// that differ just by the order of it's internal elements. For example 
    /// {A, B} is the same as {B, A}, or { {A, C}, {B, D} } is the same
    /// as { {D, B}, {A, C} }. This is the key part of constructing objects 
    /// with mutually distinct signatures. It implements the <see cref="IEnumerable{T}"/>
    /// interface, where T is the read-only list of <see cref="ConstructionArgument"/>.
    /// </summary>
    internal interface IArgumentsListContainer : IEnumerable<IReadOnlyList<ConstructionArgument>>
    {
        /// <summary>
        /// Adds an argument list to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        void Add(IReadOnlyList<ConstructionArgument> arguments);
    }
}