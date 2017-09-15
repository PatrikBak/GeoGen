using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.IdsFixing
{
    interface IIdsFixerFactory
    {
        IIdsFixer CreateFixer(DictionaryObjectIdResolver resolver);
    }
}
