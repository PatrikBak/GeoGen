using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal interface IObjectsContainersFactory
    {
        IObjectsContainer CreateContainer(IEnumerable<LooseConfigurationObject> looseObjects);
    }
}