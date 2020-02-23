using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a <see cref="ConfigurationObject"/> that is meant to be a general independent object
    /// that is passed within <see cref="ConstructionArgument"/>s to create more complex objects of the type
    /// <see cref="ConstructedConfigurationObject"/>. It is defined by a <see cref="ConfigurationObjectType"/>.
    /// </summary>
    public class LooseConfigurationObject : ConfigurationObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseConfigurationObject"/> with a given type.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        public LooseConfigurationObject(ConfigurationObjectType objectType)
            : base(objectType)
        {
        }

        #endregion

        #region Public abstract methods implementation

        /// <summary>
        /// Recreates the object using a given mapping of loose objects.
        /// </summary>
        /// <param name="mapping">The mapping of the loose objects.</param>
        /// <returns>The remapped object.</returns>
        public override ConfigurationObject Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
        {
            // Simply access the object from the mapping (there's no need to recreate it)
            return mapping.GetValueOrDefault(this) ?? throw new GeoGenException("The loose object is not present in the mapping");
        }

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <summary>
        /// Converts the loose configuration object to a string. 
        /// </summary>
        /// <returns>A human-readable string representation of the object.</returns>
        public override string ToString() => $"{ObjectType}({Id})";

#endif

        #endregion
    }
}