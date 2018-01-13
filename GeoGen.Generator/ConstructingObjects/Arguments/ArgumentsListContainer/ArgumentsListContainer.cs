using System.Collections.Generic;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsListContainer" />. 
    /// This implementation uses <see cref="StringBasedContainer{T}"/>, so
    /// all arguments are converted to string. This conversion is done
    /// using <see cref="IDefaultArgumentsListToStringConverter"/>, that uses
    /// short-signature of an argument. In this signature, objects are converted
    /// to string by simply converting its id to string. Since we eliminate 
    /// equal points on go, we don't need to use the full object representation 
    /// (which uses only loose object's ids and reveals the way that objects are constructed).
    /// </summary>
    internal class ArgumentsListContainer : StringBasedContainer<IReadOnlyList<ConstructionArgument>>, IArgumentsListContainer
    {
        #region Constructor

        /// <summary>
        /// Constructs an arguments list container that internally converts
        /// elements to string using a given converter of lists of construction arguments.
        /// </summary>
        /// <param name="converter">The converter.</param>
        public ArgumentsListContainer(IDefaultArgumentsListToStringConverter converter)
                : base(converter)
        {
        }

        #endregion

        #region IArgumentsListContainer implementation

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