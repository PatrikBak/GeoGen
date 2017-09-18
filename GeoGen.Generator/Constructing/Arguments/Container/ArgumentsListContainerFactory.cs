using System;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.Constructing.Arguments.Container
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsListContainerFactory"/> that
    /// creates instances of <see cref="ArgumentsListContainer"/>.
    /// </summary>
    internal class ArgumentsListContainerFactory : IArgumentsListContainerFactory
    {
        #region Private fields

        /// <summary>
        /// The arguments list to string provider.
        /// </summary>
        private readonly IArgumentsListToStringProvider _argumentsListToStringProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new arguments container factory from a given arguments
        /// list to string provider.
        /// </summary>
        /// <param name="argumentsListToStringProvider">The arguments list to string provider.</param>
        public ArgumentsListContainerFactory(IArgumentsListToStringProvider argumentsListToStringProvider)
        {
            _argumentsListToStringProvider = argumentsListToStringProvider ?? throw new ArgumentNullException(nameof(argumentsListToStringProvider));
        }

        #endregion

        #region IArgumentsListContainerFactory implementation

        /// <summary>
        /// Creates an empty arguments container.
        /// </summary>
        /// <returns>The arguments container.</returns>
        public IArgumentsListContainer CreateContainer()
        {
            return new ArgumentsListContainer(_argumentsListToStringProvider);
        } 

        #endregion
    }
}