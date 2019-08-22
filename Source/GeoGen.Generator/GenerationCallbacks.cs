using GeoGen.Core;
using System;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents callbacks that are called during the generation process to alter it.
    /// Their primary purpose is to rule out objects and configurations that cannot 
    /// be constructed.
    /// </summary>
    public class GenerationCallbacks
    {
        /// <summary>
        /// Gets or sets the function that is applied on a <see cref="GeneratedConfiguration"/>
        /// and <see cref="Construction"/> before this construction is used to generate
        /// new objects to the configuration. It should return true if and only if it should be applied.
        /// </summary>
        public Func<GeneratedConfiguration, Construction, bool> ConstructionFilter { get; set; }

        /// <summary>
        /// Gets or sets the function that is applied on a <see cref="ConstructedConfigurationObject"/>
        /// that is about to be added to a given configuration. It should return true if and only if
        /// this object is correct and should be added to the configuration.
        /// </summary>
        public Func<GeneratedConfiguration, ConstructedConfigurationObject, bool> ObjectsFilter { get; set; }

        /// <summary>
        /// Gets or sets the function that is applied on a valid <see cref="GeneratedConfiguration"/>.
        /// It should return true if and only if this configuration should be extended in the next iteration.
        /// </summary>
        public Func<GeneratedConfiguration, bool> ConfigurationsFilter { get; set; }
    }
}