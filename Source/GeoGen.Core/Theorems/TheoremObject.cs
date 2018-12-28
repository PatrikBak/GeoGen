using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object that wraps one or several <see cref="ConfigurationObject"/>s representing
    /// a single geometric object used in the definition of a <see cref="Theorem"/>. The semantics of
    /// the internal configuration objects is defined by <see cref="TheoremObjectSignature"/>.
    /// </summary>
    public class TheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the signature of the object.
        /// </summary>
        public TheoremObjectSignature Signature { get; }

        /// <summary>
        /// Gets the actual configuration objects that define this theorem object.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> InternalObjects { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObject"/> class.
        /// </summary>
        /// <param name="signature">The signature of the object.</param>
        /// <param name="objects">The actual configuration objects that define this theorem object.</param>
        public TheoremObject(TheoremObjectSignature signature, IEnumerable<ConfigurationObject> objects)
        {
            Signature = signature;
            InternalObjects = objects?.ToList() ?? throw new ArgumentNullException(nameof(objects));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremObject"/> class
        /// representing a wrapper for a single object, i.e. a theorem object with the
        /// <see cref="TheoremObjectSignature.SingleObject"/> signature.
        /// </summary>
        /// <param name="configurationObject">The object that defines this theorem object.</param>
        public TheoremObject(ConfigurationObject configurationObject)
            : this(TheoremObjectSignature.SingleObject, new[] { configurationObject })
        {
        }

        #endregion
    }
}