using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="ConfigurationObject"/> that is meant to be a general independent object
    /// that is passed within <see cref="ConstructionArgument"/>s to create more complex objects of the type
    /// <see cref="ConstructedConfigurationObject"/>. It is defined by a <see cref="ConfigurationObjectType"/>.
    /// </summary>
    public class LooseConfigurationObject : ConfigurationObject
    {
        #region Configuration object properties

        /// <summary>
        /// Gets the actual geometrical type of this object (such as Point, Line...)
        /// </summary>
        public override ConfigurationObjectType ObjectType { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="objectType">The geometrical type of this object.</param>
        public LooseConfigurationObject(ConfigurationObjectType objectType)
        {
            ObjectType = objectType;
        }

        #endregion

        #region Public abstract methods overrides

        /// <summary>
        /// Enumerates the objects that are internally used to create this object. The order of this objects
        /// should match the order in which we can gradually construct them.
        /// </summary>
        /// <returns>A lazy enumerable of the internal objects.</returns>
        public override IEnumerable<ConfigurationObject> InternalObjects() => Enumerable.Empty<ConfigurationObject>();

        #endregion

        #region Protected abstract methods overrides

        /// <summary>
        /// Converts the object to a string using already set names of the objects.
        /// </summary>
        /// <param name="objectToStringMap"></param>
        /// <returns>A human-readable string representation of the object.</returns>
        protected override string ToString(IReadOnlyDictionary<ConfigurationObject, string> objectToStringMap) => $"{ObjectType}[{Id}]";

        #endregion
    }
}