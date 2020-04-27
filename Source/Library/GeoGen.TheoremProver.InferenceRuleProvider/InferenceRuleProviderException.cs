using System;

namespace GeoGen.TheoremProver.InferenceRuleProvider
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the inference rule provider module.
    /// </summary>
    public class InferenceRuleProviderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRuleProviderException"/> class.
        /// </summary>
        public InferenceRuleProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRuleProviderException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public InferenceRuleProviderException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InferenceRuleProviderException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public InferenceRuleProviderException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
