using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;

namespace GeoGen.Generator.ConfigurationHandling.SymetricConfigurationsHandler
{
    interface ISymetricConfigurationsHandler
    {
        ConfigurationWrapper CreateSymetryClassRepresentant(ConfigurationWrapper configuration, List<ConstructedConfigurationObject> newObjects);
    }
}
