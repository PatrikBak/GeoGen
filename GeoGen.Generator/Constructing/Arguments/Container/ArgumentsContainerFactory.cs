using System;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.Constructing.Arguments.Container
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsContainerFactory"/> that
    /// creates instances of <see cref="ArgumentsContainer"/>.
    /// </summary>
    internal class ArgumentsContainerFactory : IArgumentsContainerFactory
    {
        #region Private fields

        /// <summary>
        /// The arguments to string provider.
        /// </summary>
        private readonly IArgumentsToStringProvider _argumentsToStringProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new arguments container factory from a given arguments
        /// to string provider.
        /// </summary>
        /// <param name="argumentsToStringProvider">The arguments to string provider.</param>
        public ArgumentsContainerFactory(IArgumentsToStringProvider argumentsToStringProvider)
        {
            _argumentsToStringProvider = argumentsToStringProvider ?? throw new ArgumentNullException(nameof(argumentsToStringProvider));
        }

        #endregion

        #region IArgumentsContainerFactory implementation

        /// <summary>
        /// Creates an empty arguments container.
        /// </summary>
        /// <returns>The arguments container.</returns>
        public IArgumentsContainer CreateContainer()
        {
            return new ArgumentsContainer(_argumentsToStringProvider);
        } 

        #endregion
    }
}