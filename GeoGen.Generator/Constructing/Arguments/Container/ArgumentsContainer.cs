using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities.StringBasedContainer;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.Constructing.Arguments.Container
{
    /// <summary>
    /// An implementation of <see cref="IArgumentsContainer"/> that uses <see cref="IArgumentsToStringProvider"/>.
    /// It inherits from <see cref="StringBasedContainer{T}"/>.
    /// </summary>
    internal class ArgumentsContainer : StringBasedContainer<IReadOnlyList<ConstructionArgument>>, IArgumentsContainer
    {
        #region StringBasedContainer properties

        /// <summary>
        /// Gets the function that converts arguments to string.
        /// </summary>
        public override Func<IReadOnlyList<ConstructionArgument>, string> ItemToString { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new arguments container that uses a given argument to string provider.
        /// </summary>
        /// <param name="argumentsToStringProvider">The argument to string provider.</param>
        public ArgumentsContainer(IArgumentsToStringProvider argumentsToStringProvider)
        {
            if (argumentsToStringProvider == null)
                throw new ArgumentNullException(nameof(argumentsToStringProvider));

            ItemToString = argumentsToStringProvider.ConvertToString;
        }

        #endregion

        #region IArguments container implementation

        /// <summary>
        /// Removes all the elemenets contained in a given container
        /// from this container. 
        /// </summary>
        /// <param name="elementsToBeRemoved">The container of elements to be removed.</param>
        public void RemoveElementsFrom(IArgumentsContainer elementsToBeRemoved)
        {
            if (elementsToBeRemoved == null)
                throw new ArgumentNullException(nameof(elementsToBeRemoved));

            var argumentsContainer = elementsToBeRemoved as ArgumentsContainer ?? throw new GeneratorException("Unhandled case");

            foreach (var item in argumentsContainer.Items)
            {
                Items.Remove(item);
            }
        }

        #endregion
    }
}