using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using GeoGen.Utilities;

namespace GeoGen.Generator.ConstructingObjects.Arguments.Container
{
    /// <summary>
    /// An implementation of <see cref="T:GeoGen.Generator.ConstructingObjects.Arguments.Container.IArgumentsListContainer" /> that 
    /// uses <see cref="T:GeoGen.Core.Utilities.StringBasedContainer`1" />, where T is the list of 
    /// <see cref="T:GeoGen.Core.Constructions.Arguments.ConstructionArgument" />, together with 
    /// <see cref="T:GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString.IArgumentsListToStringProvider" />. and the default 
    /// configuration object to string provider. Since we eliminate 
    /// equal points on go, we don't need to use the full object as
    /// string representation (that uses only loose object's ids).
    /// </summary>
    internal sealed class ArgumentsListContainer : StringBasedContainer<IReadOnlyList<ConstructionArgument>>, IArgumentsListContainer
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