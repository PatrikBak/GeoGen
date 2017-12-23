using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal interface IGeometryRegistrar
    {
        RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects);
    }
}