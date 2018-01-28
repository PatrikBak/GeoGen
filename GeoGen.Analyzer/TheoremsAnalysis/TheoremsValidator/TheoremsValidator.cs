using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="ITheoremsValidator"/>.
    /// </summary>
    internal class TheoremsValidator : ITheoremsValidator
    {
        #region Private fields

        /// <summary>
        /// The constructor of theorems objects.
        /// </summary>
        private readonly ITheoremConstructor _constructor;

        /// <summary>
        /// The container of default theorems.
        /// </summary>
        private readonly ITheoremsContainer _container;

        /// <summary>
        /// The validator of an output of verifiers.
        /// </summary>
        private readonly IOutputValidator _validator;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="constructor">The constructor of theorems.</param>
        /// <param name="container">The container of default theorems.</param>
        /// <param name="validator">The validator of verifiers output.</param>
        public TheoremsValidator(ITheoremConstructor constructor, ITheoremsContainer container, IOutputValidator validator)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        #endregion

        #region ITheoremsValidator implementation

        /// <summary>
        /// Performs the theorems validation.
        /// </summary>
        /// <param name="configuration">The configuration for which the output is found.</param>
        /// <param name="verifiersOutput">The output of all theorems verifiers.</param>
        /// <returns>The theorems.</returns>
        public IEnumerable<Theorem> ValidateTheorems(Configuration configuration, IEnumerable<VerifierOutput> verifiersOutput)
        {
            // For all output which pases validation
            return verifiersOutput.Where(output => _validator.Validate(configuration, output))
                    // Construct a theorem
                    .Select(output => _constructor.Construct(output.InvoldedObjects, output.Type))
                    // And look to the container if it's not default
                    .Where(theorem => !_container.Contains(theorem));
        } 

        #endregion
    }
}