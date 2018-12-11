using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// A servise that validates the initial and newly created configuration.
    /// This validation should include checking whether the configuration contains 
    /// duplicit and inconstructible objects.
    /// </summary>
    public interface IConfigurationsValidator
    {
        /// <summary>
        /// Validates the configuration, which means checking if it contains duplicate or inconstructible objects.
        /// </summary>
        /// <param name="configuration">The configuration to be validated.</param>
        /// <returns>true, if the configuration is valid, false otherwise</returns>
        bool Validate(Configuration configuration);
    }
}