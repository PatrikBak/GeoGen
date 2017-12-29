using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer
{
    public interface IGeometryRegistrar
    {
        RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects);
    }
}