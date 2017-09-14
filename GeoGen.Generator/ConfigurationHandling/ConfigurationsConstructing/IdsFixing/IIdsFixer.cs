using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing
{
    internal interface IIdsFixer
    {
        ConstructionArgument FixArgument(ConstructionArgument argument, DictionaryObjectIdResolver resolver);

        ConfigurationObject FixObject(ConfigurationObject configurationObject, DictionaryObjectIdResolver resolver);
    }
}