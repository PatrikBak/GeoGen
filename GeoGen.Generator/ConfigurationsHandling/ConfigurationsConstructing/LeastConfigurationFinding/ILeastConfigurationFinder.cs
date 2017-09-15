using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.LeastConfigurationFinding
{
    internal interface ILeastConfigurationFinder
    {
        DictionaryObjectIdResolver FindLeastConfiguration(Configuration configuration);
    }
}
