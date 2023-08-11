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

        /// <inheritdoc/>
        public override LooseConfigurationObject Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping)
            // Simply access the object from the mapping
            => mapping.GetValueOrDefault(this) ?? throw new GeoGenException("The loose object is not present in the mapping");

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <inheritdoc/>
        public override string ToString() => $"{ObjectType}({Id})";

#endif

        #endregion
    }
}