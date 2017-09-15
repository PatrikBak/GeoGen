using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving
{
    internal interface IDictionaryObjectIdResolversContainer : IEnumerable<DictionaryObjectIdResolver>
    {
        void Initialize(IEnumerable<LooseConfigurationObject> looseConfigurationObjects);
    }
}
