using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Utilities;
using GeoGen.Utilities.DataStructures;

namespace GeoGen.Generator
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
        #region Constructor

        /// <summary>
        /// Constructs an arguments list container that internally uses 
        /// a given converter of list of construction arguments.
        /// </summary>
        /// <param name="converter">The converter.</param>
        public ArgumentsListContainer(IDefaultArgumentsListToStringConverter converter)
                : base(converter)
        {
        }

        #endregion

        #region IArguments container methods

        /// <summary>
        /// Adds an argument list to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public new void Add(IReadOnlyList<ConstructionArgument> arguments)
        {
            // Call the base add method and ignore it's result
            base.Add(arguments);
        }

        #endregion
    }
}