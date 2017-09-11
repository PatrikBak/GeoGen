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
        #region Private fields

        /// <summary>
        /// The arguments to string provider.
        /// </summary>
        private readonly IArgumentsToStringProvider _argumentsToStringProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new arguments container that uses a given argument to string provider.
        /// </summary>
        /// <param name="argumentsToStringProvider">The argument to string provider.</param>
        public ArgumentsContainer(IArgumentsToStringProvider argumentsToStringProvider)
        {
            _argumentsToStringProvider = argumentsToStringProvider ?? throw new ArgumentNullException(nameof(argumentsToStringProvider));
        }

        #endregion

        #region IArguments container implementation

        /// <summary>
        /// Adds arguments to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public void AddArguments(IReadOnlyList<ConstructionArgument> arguments)
        {
            Add(arguments);
        }

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
                Items.Remove(item.Key);
            }
        }

        #endregion

        #region StringBasedContainer methods

        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The given item.</param>
        /// <returns>The string representation.</returns>
        protected override string ItemToString(IReadOnlyList<ConstructionArgument> item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return _argumentsToStringProvider.ConvertToString(item);
        }

        #endregion
    }
}