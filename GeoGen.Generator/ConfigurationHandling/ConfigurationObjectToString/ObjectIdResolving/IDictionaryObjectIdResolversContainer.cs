using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving
{
    internal interface IDictionaryObjectIdResolversContainer : IEnumerable<DictionaryObjectIdResolver>
    {
        void Initialize(IEnumerable<LooseConfigurationObject> looseConfigurationObjects);
    }
}
