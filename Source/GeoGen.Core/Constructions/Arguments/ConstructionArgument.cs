using System.Collections.Generic;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a holder of <see cref="ConfigurationObject"/>(s) that are used in <see cref="Arguments"/>
    /// to represents an input for <see cref="Construction"/> to get <see cref="ConstructedConfigurationObject"/>s.
    /// </summary>
    public abstract class ConstructionArgument
    {
        /// <summary>
        /// Recreates the argument using a given mapping of loose objects.
        /// </summary>
        /// <param name="mapping">The mapping of the loose objects.</param>
        /// <returns>The remapped argument.</returns>
        public abstract ConstructionArgument Remap(IReadOnlyDictionary<LooseConfigurationObject, LooseConfigurationObject> mapping);
    }
}