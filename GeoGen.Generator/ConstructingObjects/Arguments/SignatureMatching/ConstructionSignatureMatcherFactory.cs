using System;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;

namespace GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching
{
    /// <summary>
    /// A default implementation of <see cref="IConstructionSignatureMatcherFactory"/>.
    /// </summary>
    internal class ConstructionSignatureMatcherFactory : IConstructionSignatureMatcherFactory
    {
        #region Private fields

        /// <summary>
        /// The argument container.
        /// </summary>
        private readonly IArgumentContainer _argumentContainer;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new construction signature matcher factory
        /// with a given argument container.
        /// </summary>
        /// <param name="argumentContainer">The argument container.</param>
        public ConstructionSignatureMatcherFactory(IArgumentContainer argumentContainer)
        {
            _argumentContainer = argumentContainer ?? throw new ArgumentNullException(nameof(argumentContainer));
        }

        #endregion

        #region IConstructionSignatureMatcherFactory implementation

        /// <summary>
        /// Creates a construction signature matcher.
        /// </summary>
        /// <returns>The construction signature matcher.</returns>
        public IConstructionSignatureMatcher CreateMatcher()
        {
            // We need to create new instances of this class, because it's not thread-safe
            return new ConstructionSignatureMatcher(_argumentContainer);
        }

        #endregion
    }
}