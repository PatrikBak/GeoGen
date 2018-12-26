namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that validates <see cref="GeneratedConfiguration"/>s, i.e. if they
    /// are suitable for extending or finding theorems.
    /// </summary>
    public interface IConfigurationsValidator
    {
        /// <summary>
        /// Perform the validation of a given configuration. The internal objects of the configuration
        /// must be correctly identified.
        /// </summary>
        /// <param name="configuration">The configuration to be validated.</param>
        /// <returns>true, if the configuration is valid; false otherwise.</returns>
        bool Validate(GeneratedConfiguration configuration);
    }
}