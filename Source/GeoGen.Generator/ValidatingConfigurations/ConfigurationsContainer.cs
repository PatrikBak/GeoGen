using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IContainer{T}{T}"/>, where 'T' is <see cref="GeneratedConfiguration"/>, 
    /// that is not only able to recognize  equal configurations (i.e. ones who contain the same 
    /// objects in different orders), but also isomorphic configurations (for the details see the 
    /// documentation of <see cref="FullConfigurationToStringConverter"/>). This class makes use of 
    /// <see cref="StringBasedContainer{T}"/> together with <see cref="FullConfigurationToStringConverter"/>.
    /// </summary>
    public class ConfigurationsContainer : StringBasedContainer<GeneratedConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationsContainer"/> class.
        /// </summary>
        /// <param name="converter">The converter of configurations to a string used by the container.</param>
        public ConfigurationsContainer(FullConfigurationToStringConverter converter)
            : base(converter)
        {
        }
    }
}
