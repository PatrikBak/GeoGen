using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IArgumentsContainer" />. 
    /// This implementation uses <see cref="StringBasedContainer{T}"/>, so
    /// all arguments are converted to string. This conversion is done
    /// using <see cref="IDefaultArgumentsToStringConverter"/>, that uses
    /// short-signature of an argument. In this signature, objects are converted
    /// to string by simply converting its id to string. Since we eliminate 
    /// equal points on go, we don't need to use the full object representation 
    /// (which uses only loose object's ids and reveals the way that objects are constructed).
    /// </summary>
    internal class ArgumentsContainer : StringBasedContainer<Arguments>, IArgumentsContainer
    {
        #region Constructor

        /// <summary>
        /// Constructs an arguments container that internally converts
        /// elements to string using a given default converter of arguments
        /// </summary>
        /// <param name="converter">The converter.</param>
        public ArgumentsContainer(IDefaultArgumentsToStringConverter converter)
                : base(converter)
        {
        }

        #endregion

        #region IArgumentsListContainer implementation

        /// <summary>
        /// Adds arguments to the container.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public new void Add(Arguments arguments)
        {
            // Call the base add method and ignore it's result
            base.Add(arguments);
        }

        #endregion
    }
}