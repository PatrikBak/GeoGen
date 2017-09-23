using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities.StringBasedContainer;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;

namespace GeoGen.Generator.ConstructingObjects.Arguments.Container
{
    /// <summary>
    /// An implementation of <see cref="IArgumentsListContainer"/> that 
    /// uses <see cref="StringBasedContainer{T}"/>, where T is the list of 
    /// <see cref="ConstructionArgument"/>, together with 
    /// <see cref="IArgumentsListToStringProvider"/>. and the default 
    /// configuration object to string provider. Since we eliminate 
    /// equal points on go, we don't need to use the full object as
    /// string representation (that uses only loose object's ids).
    /// </summary>
    internal class ArgumentsListContainer : StringBasedContainer<IReadOnlyList<ConstructionArgument>>, IArgumentsListContainer
    {
        #region Private fields

        /// <summary>
        /// The arguments list to string provider.
        /// </summary>
        private readonly IArgumentsListToStringProvider _argumentsListToStringProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new arguments container that uses a given 
        /// arguments list to string provider for comparing arguments.
        /// </summary>
        /// <param name="argumentsListToStringProvider">The arguments list to string provider.</param>
        public ArgumentsListContainer(IArgumentsListToStringProvider argumentsListToStringProvider)
        {
            _argumentsListToStringProvider = argumentsListToStringProvider ?? throw new ArgumentNullException(nameof(argumentsListToStringProvider));
        }

        #endregion

        #region IArguments container methods

        /// <summary>
        /// Adds an argument list to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public void AddArguments(IReadOnlyList<ConstructionArgument> arguments)
        {
            // Call the base add method and ignore it's result
            Add(arguments);
        }

        /// <summary>
        /// Removes all the elements contained in a given container
        /// from this container. 
        /// </summary>
        /// <param name="elementsToBeRemoved">The container of elements to be removed.</param>
        public void RemoveElementsFrom(IArgumentsListContainer elementsToBeRemoved)
        {
            if (elementsToBeRemoved == null)
                throw new ArgumentNullException(nameof(elementsToBeRemoved));

            // We assume the other container is also an instance of this class.
            var argumentsContainer = (ArgumentsListContainer) elementsToBeRemoved;

            // And so we can access to the protected items dictionary of the base class
            foreach (var item in argumentsContainer.Items)
            {
                // And remove all its items from this items dictionary
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
            return _argumentsListToStringProvider.ConvertToString(item);
        }

        #endregion
    }
}