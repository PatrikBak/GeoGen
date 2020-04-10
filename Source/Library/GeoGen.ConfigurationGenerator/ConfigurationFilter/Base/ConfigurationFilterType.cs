namespace GeoGen.ConfigurationGenerator
{
    /// <summary>
    /// Represents the type of <see cref="IConfigurationFilter"/>.
    /// </summary>
    public enum ConfigurationFilterType
    {
        /// <summary>
        /// Represents the filter that uses O(1) memory, but is slower than <see cref="FastConfigurationFilter"/>.
        /// </summary>
        MemoryEfficient,

        /// <summary>
        /// Represents the filter that uses lots of memory, but is faster than <see cref="MemoryEfficientConfigurationFilter"/>.
        /// </summary>
        Fast,

        /// <summary>
        /// The simple filter that does not handle configuration isomorphism at all, only configuration equality.
        /// </summary>
        Simple
    }
}
