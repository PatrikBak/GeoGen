using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a container of <see cref="ConfigurationWrapper"/>s. It should contain
    /// each configuration at most once. It implements the <see cref="IEnumerable{T}"/>
    /// interface, where T is a <see cref="ConfigurationWrapper"/>.
    /// </summary>
    internal interface IConfigurationsContainer : IEnumerable<ConfigurationWrapper>
    {
        /// <summary>
        /// Adds a given configuration wrapper to the container.
        /// </summary>
        /// <param name="wrapper">The configuration wrapper.</param>
        /// <returns>true, if the content has changed; false otherwise</returns>
        bool Add(ConfigurationWrapper wrapper);

        /// <summary>
        /// Clears the content of the container.
        /// </summary>
        void Clear();
    }
}