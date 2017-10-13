using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Constructing
{
    internal interface IObjectsConstructor
    {
        ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects);
    }
}
