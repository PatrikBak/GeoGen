using GeoGen.Core;

namespace GeoGen.ConfigurationGenerator
{
    /// <summary>
    /// Represents a service is able to detect whether a given <see cref="Configuration"/> should be excluded
    /// during the generation process, for example, to ensure that we won't generate the same configuration twice.
    /// </summary>
    public interface IConfigurationFilter
    {
        /// <summary>
        /// The type of the filter.
        /// </summary>
        ConfigurationFilterType Type { get; }

        /// <summary>
        /// Finds out if the configuration should be excluded by the algorithm, because it is 
        /// not the representant of the equivalence class of equal configurations.
        /// </summary>
        /// <param name="configuration">The configuration that should be tested for exclusion.</param>
        /// <returns>true, if the configuration should be excluded; false otherwise.</returns>
        bool ShouldBeExcluded(Configuration configuration);
    }
}