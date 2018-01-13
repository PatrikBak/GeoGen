using System.Collections.Generic;
using GeoGen.Analyzer;
using GeoGen.Core;

namespace GeoGen.Generator.IntegrationTest
{
    internal class DummyGeometryRegistrar : IGeometryRegistrar
    {
        public RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects)
        {
            return RegistrationResult.Ok;
        }
    }
}