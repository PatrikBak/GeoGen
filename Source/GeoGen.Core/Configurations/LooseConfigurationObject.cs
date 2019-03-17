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
        /// <summary>
        /// Initializes a new instance of the <see cref="LooseConfigurationObject"/> with a given type.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        public LooseConfigurationObject(ConfigurationObjectType objectType)
            : base(objectType)
        {
        }
    }
}