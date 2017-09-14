using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.LeastConfigurationFinding
{
    internal interface ILeastConfigurationFinder
    {
        DictionaryObjectIdResolver FindLeastConfiguration(Configuration configuration);
    }
}
