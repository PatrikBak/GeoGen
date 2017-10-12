using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal interface IObjectsContainersHolder : IEnumerable<IObjectsContainer>
    {
        void Initialize(IEnumerable<LooseConfigurationObject> looseObjects);
    }
}