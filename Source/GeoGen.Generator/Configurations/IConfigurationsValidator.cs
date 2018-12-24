namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that validates if a given configuration should be tested for theorems
    /// and extended further. 
    /// </summary>
    public interface IConfigurationsValidator
    {
        /// <summary>
        /// Perform the validation on a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration to be validated.</param>
        /// <returns>true, if the configuration is valid, false otherwise</returns>
        bool Validate(GeneratedConfiguration configuration);
    }
}