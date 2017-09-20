using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.ConstructingObjects.Arguments.Container
{
    /// <summary>
    /// Represents a container that is able to recognize the same arguments that differ just by the order
    /// of it's internal elements. For example {A, B} is the same as {B, A}, or { {A, C}, {B, D} } is the same
    /// as { {D, B}, {A, C} }. This is the key part of constructing objects with mutually distinct signatures.
    /// </summary>
    internal interface IArgumentsListContainer : IEnumerable<IReadOnlyList<ConstructionArgument>>
    {
        /// <summary>
        /// Adds arguments to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        void AddArguments(IReadOnlyList<ConstructionArgument> arguments);

        /// <summary>
        /// Removes all the elements contained in a given container
        /// from this container. 
        /// </summary>
        /// <param name="elementsToBeRemoved">The container of elements to be removed.</param>
        void RemoveElementsFrom(IArgumentsListContainer elementsToBeRemoved);
    }
}