using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    internal interface IGeometryRegistrar
    {
        void Initialize(Configuration configuration);

        RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects);
    }
}