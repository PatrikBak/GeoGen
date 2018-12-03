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
    /// interface, where T is <see cref="Arguments"/>.
    /// </summary>
    internal interface IArgumentsContainer : IEnumerable<Arguments>
    {
        /// <summary>
        /// Adds arguments to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        void Add(Arguments arguments);
    }
}