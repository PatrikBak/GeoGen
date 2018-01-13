using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents an object that are used to define <see cref="Theorem"/>.
    /// </summary>
    public class TheoremObject
    {
        #region Public properties

        /// <summary>
        /// Gets the type of the signature that this object has.
        /// </summary>
        public TheoremObjectSignature Type { get; }

        /// <summary>
        /// Gets the actual configuration objects that defines this object.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> InternalObjects { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type">The signature type that this object has.</param>
        /// <param name="objects">The list of objects that define this theorem object.</param>
        public TheoremObject(TheoremObjectSignature type, IReadOnlyList<ConfigurationObject> objects)
        {
            Type = type;
            InternalObjects = objects ?? throw new ArgumentNullException(nameof(objects));
        }

        /// <summary>
        /// Constructor for a theorem object wrapping a single configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        public TheoremObject(ConfigurationObject configurationObject)
        {
            Type = TheoremObjectSignature.SingleObject;
            InternalObjects = new List<ConfigurationObject> {configurationObject};
        }

        #endregion
    }
}