using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a <see cref="IContainer{T}"/>, where 'T' is <see cref="ConfigurationObject"/>,
    /// that is able to recognize equal configuration objects, i.e. the ones who are formally
    /// equal (for example, the midpoint of AB is equal to the midpoint of BA). This container
    /// uses <see cref="FullObjectToStringConverter"/> together with no ids remapping. This class
    /// makes use of <see cref="StringBasedContainer{T}"/>.
    /// </summary>
    public class ConfigurationObjectsContainer : StringBasedContainer<ConfigurationObject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationObjectsContainer"/> class.
        /// </summary>
        /// <param name="converter">The converter of objects to a string used by the container.</param>
        public ConfigurationObjectsContainer(FullObjectToStringConverter converter) 
            : base(converter)
        {
        }
    }
}
