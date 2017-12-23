using System.Collections.Generic;

namespace GeoGen.Generator
{
    internal interface IConfigurationsContainer : IEnumerable<ConfigurationWrapper>
    {
        /// <summary>
        /// Adds a given configuration wrapper to the container.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <returns>true, if the content has changed; false otherwise</returns>
        bool Add(ConfigurationWrapper wrapper);

        /// <summary>
        /// Clears the content of the container.
        /// </summary>
        void Clear();
    }
}