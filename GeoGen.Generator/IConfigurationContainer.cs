using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    internal interface IConfigurationContainer
    {
        IEnumerable<Configuration> Configurations { get; }

        void AddNewLayer(List<Configuration> newLayerConfigurations);
    }
}
