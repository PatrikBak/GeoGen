using System;
using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="ConstructionArgument"/> that wraps a single <see cref="ConfigurationObject"/>.
    /// </summary>
    public class ObjectConstructionArgument : ConstructionArgument
    {
        #region Public properties

        /// <summary>
        /// Gets the configuration object that is passed as an argument.
        /// </summary>
        public ConfigurationObject PassedObject { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConstructionArgument"/> class.
        /// </summary>
        /// <param name="passedObject">The configuration object that is passed as an argument.</param>
        public ObjectConstructionArgument(ConfigurationObject passedObject)
        {
            PassedObject = passedObject ?? throw new ArgumentNullException(nameof(passedObject));
        }

        #endregion

        #region Public abstract methods implementation

        /// <inheritdoc/>
        public override ObjectConstructionArgument Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
            // Simply remap the passed object
            => new ObjectConstructionArgument(PassedObject.Remap(mapping));

        #endregion

        #region HashCode and Equals

        /// <inheritdoc/>
        public override int GetHashCode() => PassedObject.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object otherObject)
            // Either the references are equals
            => this == otherObject
                // Or the object is not null
                || otherObject != null
                // And is an object argument
                && otherObject is ObjectConstructionArgument objectArgument
                // And their objects are equal
                && objectArgument.PassedObject.Equals(PassedObject);

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString() => PassedObject.Id.ToString();

#endif

        #endregion
    }
}