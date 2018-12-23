using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    public class ConfigurationsContainer : AutoIdentifyingStringBasedContainer<GeneratedConfiguration>
    {
        public ConfigurationsContainer(FullConfigurationToStringConverter converter) 
            : base(converter)
        {
        }
    }
}
