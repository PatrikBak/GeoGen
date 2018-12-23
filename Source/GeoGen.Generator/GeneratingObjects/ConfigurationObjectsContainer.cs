using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    public class ConfigurationObjectsContainer : AutoIdentifyingStringBasedContainer<ConfigurationObject>
    {
        public ConfigurationObjectsContainer(Configuration initialConfiguration, IFullObjectToStringConvertersContainer container) 
            : base(container.DefaultFullConverter)
        {
            // Add the objects of the initial configuration
            initialConfiguration.ObjectsMap.AllObjects.ForEach(obj => Add(obj, out var _));
        }
    }
}
