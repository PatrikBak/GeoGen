using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.ConstructingConfigurations.IdsFixing
{
    /// <summary>
    /// Represents a service that can replace configuration  objects / arguments with their
    /// versions with new ids. 
    /// </summary>
    internal interface IIdsFixer
    {
        /// <summary>
        /// Replaces a given construction argument with a new one that
        /// has correct ids of its interior objects. 
        /// </summary>
        /// <param name="argument">The construction argument.</param>
        /// <returns>The fixed configuration argument.</returns>
        ConstructionArgument FixArgument(ConstructionArgument argument);

        /// <summary>
        /// Replaces a given configuration object with a new one that 
        /// has correct ids of its interior objects.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The fixed configuration object.</returns>
        ConfigurationObject FixObject(ConfigurationObject configurationObject);
    }
}