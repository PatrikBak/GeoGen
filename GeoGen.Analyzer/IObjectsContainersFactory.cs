using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer
{
    internal interface IObjectsContainersFactory
    {
        IObjectsContainer CreateContainer(IEnumerable<LooseConfigurationObject> looseObjects);
    }
}