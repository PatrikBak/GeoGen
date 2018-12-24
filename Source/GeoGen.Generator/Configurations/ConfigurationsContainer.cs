using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IContainer{T}"/>, where 'T' is <see cref="Configuration"/>,
    /// that is able to recognize not only equal configurations (i.e. ones who contain the same
    /// object in different orders), but also isomorphic configurations (for the details see
    /// the documentation of <see cref="FullObjectToStringConverter"/>). This container
    /// also automatically identifies the configurations that are added to it.
    /// This class makes use of <see cref="AutoIdentifyingStringBasedContainer{T}"/>.
    /// </summary>
    public class ConfigurationsContainer : AutoIdentifyingStringBasedContainer<GeneratedConfiguration>
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
