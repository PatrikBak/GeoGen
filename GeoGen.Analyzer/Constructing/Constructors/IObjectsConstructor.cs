using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Constructing.Constructors
{
    internal interface IObjectsConstructor
    {
        ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects);
    }
}
