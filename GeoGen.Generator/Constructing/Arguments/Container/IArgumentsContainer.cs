using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Constructing.Arguments.Container
{
    /// <summary>
    /// Represents a container that is able to recognize the same arguments that differ just by the order
    /// of it's internal elements. For example {A, B} is the same as {B, A}, or { {A, C}, {B, D} } is the same
    /// as { {D, B}, {A, C} }. This is the key part of constructing objects with mutually distinct signatures.
    /// </summary>
    internal interface IArgumentsContainer : IEnumerable<IReadOnlyList<ConstructionArgument>>
    {
        /// <summary>
        /// Adds arguments to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        void Add(IReadOnlyList<ConstructionArgument> arguments);

        /// <summary>
        /// Clears the container.
        /// </summary>
        void Clear();

        /// <summary>
        /// Checks if the container contains given arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>true, if the container container the arguments, false otherwise.</returns>
        bool Contains(IReadOnlyList<ConstructionArgument> arguments);
    }
}