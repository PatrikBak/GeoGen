using System;

namespace GeoGen.TheoremProver.ObjectIntroductionRuleProvider
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the object introduction rule provider module.
    /// </summary>
    public class ObjectIntroductionRuleProviderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIntroductionRuleProviderException"/> class.
        /// </summary>
        public ObjectIntroductionRuleProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIntroductionRuleProviderException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public ObjectIntroductionRuleProviderException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIntroductionRuleProviderException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public ObjectIntroductionRuleProviderException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
