using System;

namespace GeoGen.TheoremSimplifier.SimplificationRuleProvider
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the simplification rule provider module.
    /// </summary>
    public class SimplificationRuleProviderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRuleProviderException"/> class.
        /// </summary>
        public SimplificationRuleProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRuleProviderException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public SimplificationRuleProviderException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplificationRuleProviderException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public SimplificationRuleProviderException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
