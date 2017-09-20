using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding
{
    internal interface ILeastConfigurationFinder
    {
        DictionaryObjectIdResolver FindLeastConfiguration(Configuration configuration);
    }
}
