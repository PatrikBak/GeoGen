using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.IdsFixing
{
    internal interface IIdsFixer
    {
        ConstructionArgument FixArgument(ConstructionArgument argument);

        ConfigurationObject FixObject(ConfigurationObject configurationObject);
    }
}