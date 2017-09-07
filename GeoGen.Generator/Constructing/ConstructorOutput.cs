using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationHandling;

namespace GeoGen.Generator.Constructing
{
    internal class ConstructorOutput
    {
        public ConfigurationWrapper InitialConfiguration { get; }

        public List<ConstructedConfigurationObject> ConstructedObjects { get; set; }

        public ConstructorOutput(ConfigurationWrapper initialConfiguration, List<ConstructedConfigurationObject> constructedObjects)
        {
            InitialConfiguration = initialConfiguration;
            ConstructedObjects = constructedObjects;
        }
    }
}